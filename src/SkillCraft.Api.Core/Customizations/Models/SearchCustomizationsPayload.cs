using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Customizations.Models;

public record SearchCustomizationsPayload : SearchPayload
{
  public CustomizationKind? Kind { get; set; }

  public new List<CustomizationSortOption> Sort { get; set; } = [];
}
