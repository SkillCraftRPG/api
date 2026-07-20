using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Customizations.Models;

public class CustomizationModel : Aggregate
{
  public CustomizationKind Kind { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
