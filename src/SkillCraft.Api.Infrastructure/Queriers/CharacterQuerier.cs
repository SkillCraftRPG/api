using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CharacterQuerier : ICharacterQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<CharacterEntity> _characters;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CharacterQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _characters = database.Characters;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public async Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken)
  {
    return await ReadAsync(character.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'StreamId={character.Id}' was not found.");
  }
  public async Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _characters.AsNoTracking().AsSplitQuery()
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.Caste)
      .Include(x => x.Customizations).ThenInclude(x => x.Customization)
      .Include(x => x.Education)
      .Include(x => x.Languages).ThenInclude(x => x.Language).ThenInclude(x => x!.Script)
      .Include(x => x.Lineage).ThenInclude(x => x!.Languages).ThenInclude(x => x.Script)
      .Include(x => x.Lineage).ThenInclude(x => x!.Parent).ThenInclude(x => x!.Languages).ThenInclude(x => x.Script)
      .Include(x => x.Talents).ThenInclude(x => x.Talent)
      .SingleOrDefaultAsync(cancellationToken);

    return character is null ? null : await MapAsync(character, cancellationToken);
  }
  public async Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _characters.AsNoTracking().AsSplitQuery()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .Include(x => x.Caste)
      .Include(x => x.Customizations).ThenInclude(x => x.Customization)
      .Include(x => x.Education)
      .Include(x => x.Languages).ThenInclude(x => x.Language).ThenInclude(x => x!.Script)
      .Include(x => x.Lineage).ThenInclude(x => x!.Languages).ThenInclude(x => x.Script)
      .Include(x => x.Lineage).ThenInclude(x => x!.Parent).ThenInclude(x => x!.Languages).ThenInclude(x => x.Script)
      .Include(x => x.Talents).ThenInclude(x => x.Talent)
      .SingleOrDefaultAsync(cancellationToken);

    return character is null ? null : await MapAsync(character, cancellationToken);
  }

  public async Task<SearchResults<CharacterModel>> SearchAsync(SearchCharactersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Characters.Table).SelectAll(Db.Characters.Table)
      .Where(Db.Characters.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Characters.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Characters.Name);

    if (payload.LineageId.HasValue)
    {
      OperatorCondition condition = new(Db.Lineages.Id, Operators.IsEqualTo(payload.LineageId.Value));
      builder.Join(Db.Lineages.LineageId, Db.Characters.LineageId, condition);
    }
    if (payload.CasteId.HasValue)
    {
      OperatorCondition condition = new(Db.Castes.Id, Operators.IsEqualTo(payload.CasteId.Value));
      builder.Join(Db.Castes.CasteId, Db.Characters.CasteId, condition);
    }
    if (payload.EducationId.HasValue)
    {
      OperatorCondition condition = new(Db.Educations.Id, Operators.IsEqualTo(payload.EducationId.Value));
      builder.Join(Db.Educations.EducationId, Db.Characters.EducationId, condition);
    }

    IQueryable<CharacterEntity> query = _characters.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CharacterEntity>? ordered = null;
    foreach (CharacterSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CharacterSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CharacterSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CharacterSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    CharacterEntity[] characters = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CharacterModel> items = await MapAsync(characters, cancellationToken);

    return new SearchResults<CharacterModel>(items, total);
  }

  private async Task<CharacterModel> MapAsync(CharacterEntity character, CancellationToken cancellationToken)
  {
    return (await MapAsync([character], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CharacterModel>> MapAsync(IEnumerable<CharacterEntity> characters, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = characters.SelectMany(character => character.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    CharacterMapper mapper = new(actors);

    return characters.Select(mapper.ToCharacter).ToList().AsReadOnly();
  }
}
