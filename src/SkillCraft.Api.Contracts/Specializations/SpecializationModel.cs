using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Specializations;

public class SpecializationModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public RequirementsModel Requirements { get; set; } = new();
  public OptionsModel Options { get; set; } = new();
  public DoctrineModel? Doctrine { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
