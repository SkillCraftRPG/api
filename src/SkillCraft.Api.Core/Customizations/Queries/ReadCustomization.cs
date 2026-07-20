using Logitar.CQRS;
using SkillCraft.Api.Core.Customizations.Models;

namespace SkillCraft.Api.Core.Customizations.Queries;

internal record ReadCustomizationQuery(Guid Id) : IQuery<CustomizationModel?>;

internal class ReadCustomizationQueryHandler : IQueryHandler<ReadCustomizationQuery, CustomizationModel?>
{
  private readonly ICustomizationRepository _customizationRepository;

  public ReadCustomizationQueryHandler(ICustomizationRepository customizationRepository)
  {
    _customizationRepository = customizationRepository;
  }

  public async Task<CustomizationModel?> HandleAsync(ReadCustomizationQuery query, CancellationToken cancellationToken)
  {
    return await _customizationRepository.ReadAsync(query.Id, cancellationToken);
  }
}
