using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CustomizationRepository : Repository, ICustomizationRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CustomizationRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Customization[] customizations)
  {
    foreach (Customization customization in customizations)
    {
      Database.Customizations.Add(customization);
      base.RecordChange(new CustomizationCreated(customization));
    }
  }
  public void Remove(Customization customization)
  {
    Database.Customizations.Remove(customization);
    base.RecordChange(new CustomizationDeleted(customization, _context.UserId));
  }
  public void Update(Customization customization, CustomizationUpdated record)
  {
    Database.Customizations.Update(customization);
    base.RecordChange(record);
  }

  public async Task<Customization?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Customizations.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken)
  {
    return await ReadAsync(customization.Id, cancellationToken) ?? throw new InvalidOperationException($"The customization 'Id={customization.Id}' was not found.");
  }
  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Customization? customization = await Database.Customizations.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return customization is null ? null : await MapAsync(customization, cancellationToken);
  }

  public virtual async Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Customizations.Table).SelectAll(Db.Customizations.Table)
      .Where(Db.Customizations.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Customizations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Customizations.Name, Db.Customizations.Summary);

    if (payload.Kind.HasValue)
    {
      builder.Where(Db.Customizations.Kind, Operators.IsEqualTo(payload.Kind.Value.ToString()));
    }

    IQueryable<Customization> query = Database.Customizations.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Customization>? ordered = null;
    foreach (CustomizationSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CustomizationSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CustomizationSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CustomizationSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Customization[] customizations = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CustomizationModel> items = await MapAsync(customizations, cancellationToken);

    return new SearchResults<CustomizationModel>(items, total);
  }

  private async Task<CustomizationModel> MapAsync(Customization customization, CancellationToken cancellationToken)
  {
    return (await MapAsync([customization], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CustomizationModel>> MapAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = customizations.SelectMany(customization => customization.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return customizations.Select(mapper.ToCustomization).ToList().AsReadOnly();
  }
}
