using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class EducationQuerier : IEducationQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<EducationEntity> _educations;
  private readonly ISqlHelper _sqlHelper;

  public EducationQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _educations = database.Educations;
    _sqlHelper = sqlHelper;
  }

  public async Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken)
  {
    return await ReadAsync(education.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The education entity 'StreamId={education.Id}' was not found.");
  }
  public async Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken)
  {
    EducationEntity? education = await _educations.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return education is null ? null : await MapAsync(education, cancellationToken);
  }
  public async Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationEntity? education = await _educations.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .SingleOrDefaultAsync(cancellationToken);

    return education is null ? null : await MapAsync(education, cancellationToken);
  }

  public virtual async Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Educations.Table).SelectAll(Db.Educations.Table)
      .Where(Db.Educations.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Educations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Educations.Name, Db.Educations.Summary, Db.Educations.FeatureName);

    if (payload.Skill.HasValue)
    {
      builder.Where(Db.Educations.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<EducationEntity> query = _educations.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<EducationEntity>? ordered = null;
    foreach (EducationSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case EducationSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case EducationSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case EducationSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    EducationEntity[] educations = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<EducationModel> items = await MapAsync(educations, cancellationToken);

    return new SearchResults<EducationModel>(items, total);
  }

  private async Task<EducationModel> MapAsync(EducationEntity education, CancellationToken cancellationToken)
  {
    return (await MapAsync([education], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<EducationModel>> MapAsync(IEnumerable<EducationEntity> educations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = educations.SelectMany(education => education.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return educations.Select(mapper.ToEducation).ToList().AsReadOnly();
  }
}
