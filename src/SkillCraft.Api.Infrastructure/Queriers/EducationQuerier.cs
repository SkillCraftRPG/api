using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class EducationQuerier : IEducationQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<EducationEntity> _educations;
  private readonly ISqlHelper _sqlHelper;

  public EducationQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _educations = game.Educations;
    _sqlHelper = sqlHelper;
  }

  public async Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken)
  {
    return await ReadAsync(education.Id, cancellationToken) ?? throw new InvalidOperationException($"The education entity 'StreamId={education.Id}' was not found.");
  }
  public async Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken)
  {
    EducationEntity? education = await _educations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return education is null ? null : await MapAsync(education, cancellationToken);
  }
  public async Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationEntity? education = await _educations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return education is null ? null : await MapAsync(education, cancellationToken);
  }

  public async Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Educations.Table).SelectAll(GameDb.Educations.Table)
      .ApplyIdFilter(GameDb.Educations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Educations.Name, GameDb.Educations.Summary, GameDb.Educations.FeatureName);

    if (payload.Skill.HasValue)
    {
      builder.Where(GameDb.Educations.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<EducationEntity> query = _educations.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<EducationEntity>? ordered = null;
    foreach (EducationSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        EducationSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        EducationSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        EducationSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The education sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    EducationEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<EducationModel> educations = await MapAsync(entities, cancellationToken);

    return new SearchResults<EducationModel>(educations, total);
  }

  private async Task<EducationModel> MapAsync(EducationEntity education, CancellationToken cancellationToken)
  {
    return (await MapAsync([education], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<EducationModel>> MapAsync(IEnumerable<EducationEntity> educations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = educations.SelectMany(education => education.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return educations.Select(mapper.ToEducation).ToList().AsReadOnly();
  }
}
