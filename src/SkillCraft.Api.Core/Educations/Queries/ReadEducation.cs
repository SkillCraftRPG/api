using Logitar.CQRS;
using SkillCraft.Api.Core.Educations.Models;

namespace SkillCraft.Api.Core.Educations.Queries;

internal record ReadEducationQuery(Guid Id) : IQuery<EducationModel?>;

internal class ReadEducationQueryHandler : IQueryHandler<ReadEducationQuery, EducationModel?>
{
  private readonly IEducationRepository _educationRepository;

  public ReadEducationQueryHandler(IEducationRepository educationRepository)
  {
    _educationRepository = educationRepository;
  }

  public async Task<EducationModel?> HandleAsync(ReadEducationQuery query, CancellationToken cancellationToken)
  {
    return await _educationRepository.ReadAsync(query.Id, cancellationToken);
  }
}
