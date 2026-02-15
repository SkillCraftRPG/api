using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CustomizationQuerier : ICustomizationQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<CustomizationEntity> _customizations;
  private readonly ISqlHelper _sqlHelper;

  public CustomizationQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _customizations = game.Customizations;
    _sqlHelper = sqlHelper;
  }

  public async Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken)
  {
    return await ReadAsync(customization.Id, cancellationToken) ?? throw new InvalidOperationException($"The customization entity 'StreamId={customization.Id}' was not found.");
  }
  public async Task<CustomizationModel?> ReadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _customizations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return customization is null ? null : await MapAsync(customization, cancellationToken);
  }
  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _customizations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return customization is null ? null : await MapAsync(customization, cancellationToken);
  }

  public async Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Customizations.Table).SelectAll(GameDb.Customizations.Table)
      .ApplyIdFilter(GameDb.Customizations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Customizations.Name, GameDb.Customizations.Summary);

    if (payload.Kind.HasValue)
    {
      builder.Where(GameDb.Customizations.Kind, Operators.IsEqualTo(payload.Kind.Value));
    }

    IQueryable<CustomizationEntity> query = _customizations.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CustomizationEntity>? ordered = null;
    foreach (CustomizationSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        CustomizationSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        CustomizationSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        CustomizationSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The customization sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    CustomizationEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CustomizationModel> customizations = await MapAsync(entities, cancellationToken);

    return new SearchResults<CustomizationModel>(customizations, total);
  }

  private async Task<CustomizationModel> MapAsync(CustomizationEntity customization, CancellationToken cancellationToken)
  {
    return (await MapAsync([customization], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CustomizationModel>> MapAsync(IEnumerable<CustomizationEntity> customizations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = customizations.SelectMany(customization => customization.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return customizations.Select(mapper.ToCustomization).ToList().AsReadOnly();
  }
}
