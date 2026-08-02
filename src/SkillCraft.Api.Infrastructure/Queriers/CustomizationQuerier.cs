using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CustomizationQuerier : ICustomizationQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<CustomizationEntity> _customizations;
  private readonly ISqlHelper _sqlHelper;

  public CustomizationQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _customizations = database.Customizations;
    _sqlHelper = sqlHelper;
  }

  public async Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken)
  {
    return await ReadAsync(customization.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The customization entity 'StreamId={customization.Id}' was not found.");
  }
  public async Task<CustomizationModel?> ReadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _customizations.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return customization is null ? null : await MapAsync(customization, cancellationToken);
  }
  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _customizations.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .SingleOrDefaultAsync(cancellationToken);

    return customization is null ? null : await MapAsync(customization, cancellationToken);
  }

  public virtual async Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Customizations.Table).SelectAll(Db.Customizations.Table)
      .Where(Db.Customizations.WorldId, Operators.IsEqualTo(_context.WorldUid))
      .ApplyIdFilter(Db.Customizations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Customizations.Name, Db.Customizations.Summary);

    if (payload.Kind.HasValue)
    {
      builder.Where(Db.Customizations.Kind, Operators.IsEqualTo(payload.Kind.Value.ToString()));
    }

    IQueryable<CustomizationEntity> query = _customizations.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CustomizationEntity>? ordered = null;
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

    CustomizationEntity[] customizations = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CustomizationModel> items = await MapAsync(customizations, cancellationToken);

    return new SearchResults<CustomizationModel>(items, total);
  }

  private async Task<CustomizationModel> MapAsync(CustomizationEntity customization, CancellationToken cancellationToken)
  {
    return (await MapAsync([customization], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CustomizationModel>> MapAsync(IEnumerable<CustomizationEntity> customizations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = customizations.SelectMany(customization => customization.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return customizations.Select(mapper.ToCustomization).ToList().AsReadOnly();
  }
}
