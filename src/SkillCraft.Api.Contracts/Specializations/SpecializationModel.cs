using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Specializations;

public class SpecializationModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  // TODO(fpion): Requirements { Talent, Other }
  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
