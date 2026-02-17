using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Core.Specializations.Queries;

internal record ReadSpecializationQuery(Guid Id) : IQuery<SpecializationModel?>;

internal class ReadSpecializationQueryHandler : IQueryHandler<ReadSpecializationQuery, SpecializationModel?>
{
  private readonly ISpecializationQuerier _specializationQuerier;

  public ReadSpecializationQueryHandler(ISpecializationQuerier specializationQuerier)
  {
    _specializationQuerier = specializationQuerier;
  }

  public async Task<SpecializationModel?> HandleAsync(ReadSpecializationQuery query, CancellationToken cancellationToken)
  {
    return await _specializationQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
