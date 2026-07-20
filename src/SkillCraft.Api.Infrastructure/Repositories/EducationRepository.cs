using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class EducationRepository : Repository, IEducationRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public EducationRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Education[] educations)
  {
    foreach (Education education in educations)
    {
      Database.Educations.Add(education);
      base.RecordChange(new EducationCreated(education));
    }
  }
  public void Remove(Education education)
  {
    Database.Educations.Remove(education);
    base.RecordChange(new EducationDeleted(education, _context.UserId));
  }
  public void Update(Education education, EducationUpdated record)
  {
    Database.Educations.Update(education);
    base.RecordChange(record);
  }

  public async Task<Education?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Educations.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken)
  {
    return await ReadAsync(education.Id, cancellationToken) ?? throw new InvalidOperationException($"The education 'Id={education.Id}' was not found.");
  }
  public async Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Education? education = await Database.Educations.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return education is null ? null : await MapAsync(education, cancellationToken);
  }

  public virtual async Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Educations.Table).SelectAll(Db.Educations.Table)
      .Where(Db.Educations.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Educations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Educations.Name, Db.Educations.Summary, Db.Educations.FeatureName);

    if (payload.Skill.HasValue)
    {
      builder.Where(Db.Educations.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<Education> query = Database.Educations.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Education>? ordered = null;
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

    Education[] educations = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<EducationModel> items = await MapAsync(educations, cancellationToken);

    return new SearchResults<EducationModel>(items, total);
  }

  private async Task<EducationModel> MapAsync(Education education, CancellationToken cancellationToken)
  {
    return (await MapAsync([education], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<EducationModel>> MapAsync(IEnumerable<Education> educations, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = educations.SelectMany(education => education.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return educations.Select(mapper.ToEducation).ToList().AsReadOnly();
  }
}
