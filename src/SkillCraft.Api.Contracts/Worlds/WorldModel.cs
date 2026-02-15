using Krakenar.Contracts;
using Krakenar.Contracts.Actors;

namespace SkillCraft.Api.Contracts.Worlds;

public class WorldModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public Actor Owner { get; set; } = new();

  public override string ToString() => $"{Name} | {base.ToString()}";
}
