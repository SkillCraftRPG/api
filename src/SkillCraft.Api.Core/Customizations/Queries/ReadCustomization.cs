using Logitar.CQRS;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations.Queries;

internal record ReadCustomizationQuery(Guid Id) : IQuery<CustomizationModel?>;

internal class ReadCustomizationQueryHandler : IQueryHandler<ReadCustomizationQuery, CustomizationModel?>
{
  private readonly ICustomizationQuerier _customizationQuerier;

  public ReadCustomizationQueryHandler(ICustomizationQuerier customizationQuerier)
  {
    _customizationQuerier = customizationQuerier;
  }

  public async Task<CustomizationModel?> HandleAsync(ReadCustomizationQuery query, CancellationToken cancellationToken)
  {
    return await _customizationQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
