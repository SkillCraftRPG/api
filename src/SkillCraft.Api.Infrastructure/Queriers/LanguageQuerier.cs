using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<LanguageEntity> _languages;
  private readonly ISqlHelper _sqlHelper;

  public LanguageQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _languages = database.Languages;
    _sqlHelper = sqlHelper;
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'StreamId={language.Id}' was not found.");
  }
  public async Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .Include(x => x.Script)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return language is null ? null : await MapAsync(language, cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .Include(x => x.Script)
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .SingleOrDefaultAsync(cancellationToken);

    return language is null ? null : await MapAsync(language, cancellationToken);
  }

  public virtual async Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Languages.Table).SelectAll(Db.Languages.Table)
      .Where(Db.Languages.WorldId, Operators.IsEqualTo(_context.WorldUid))
      .ApplyIdFilter(Db.Languages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Languages.Name, Db.Languages.Summary);

    if (payload.ScriptId.HasValue)
    {
      OperatorCondition condition = new(Db.Scripts.Id, Operators.IsEqualTo(payload.ScriptId.Value));
      builder.Join(Db.Scripts.ScriptId, Db.Languages.ScriptId, condition);
    }

    IQueryable<LanguageEntity> query = _languages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Script);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LanguageEntity>? ordered = null;
    foreach (LanguageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LanguageSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LanguageSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LanguageSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    LanguageEntity[] languages = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LanguageModel> models = await MapAsync(languages, cancellationToken);

    return new SearchResults<LanguageModel>(models, total);
  }

  private async Task<LanguageModel> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToList().AsReadOnly();
  }
}
