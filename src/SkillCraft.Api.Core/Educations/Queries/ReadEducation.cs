using Logitar.CQRS;
using SkillCraft.Api.Contracts.Educations;

namespace SkillCraft.Api.Core.Educations.Queries;

internal record ReadEducationQuery(Guid Id) : IQuery<EducationModel?>;

internal class ReadEducationQueryHandler : IQueryHandler<ReadEducationQuery, EducationModel?>
{
  private readonly IEducationQuerier _educationQuerier;

  public ReadEducationQueryHandler(IEducationQuerier educationQuerier)
  {
    _educationQuerier = educationQuerier;
  }

  public async Task<EducationModel?> HandleAsync(ReadEducationQuery query, CancellationToken cancellationToken)
  {
    return await _educationQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
