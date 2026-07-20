using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CasteRepository : Repository, ICasteRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CasteRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Caste[] castes)
  {
    foreach (Caste caste in castes)
    {
      Database.Castes.Add(caste);
      base.RecordChange(new CasteCreated(caste));
    }
  }
  public void Remove(Caste caste)
  {
    Database.Castes.Remove(caste);
    base.RecordChange(new CasteDeleted(caste, _context.UserId));
  }
  public void Update(Caste caste, CasteUpdated record)
  {
    Database.Castes.Update(caste);
    base.RecordChange(record);
  }

  public async Task<Caste?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Castes.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken)
  {
    return await ReadAsync(caste.Id, cancellationToken) ?? throw new InvalidOperationException($"The caste 'Id={caste.Id}' was not found.");
  }
  public async Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Caste? caste = await Database.Castes.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return caste is null ? null : await MapAsync(caste, cancellationToken);
  }

  public virtual async Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Castes.Table).SelectAll(Db.Castes.Table)
      .Where(Db.Castes.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Castes.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Castes.Name, Db.Castes.Summary);

    if (payload.Skill.HasValue)
    {
      builder.Where(Db.Castes.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<Caste> query = Database.Castes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Caste>? ordered = null;
    foreach (CasteSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CasteSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CasteSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CasteSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Caste[] castes = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CasteModel> items = await MapAsync(castes, cancellationToken);

    return new SearchResults<CasteModel>(items, total);
  }

  private async Task<CasteModel> MapAsync(Caste caste, CancellationToken cancellationToken)
  {
    return (await MapAsync([caste], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CasteModel>> MapAsync(IEnumerable<Caste> castes, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = castes.SelectMany(caste => caste.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return castes.Select(mapper.ToCaste).ToList().AsReadOnly();
  }
}
