using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Parties;

public class PartyModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
