using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Scripts.Models;

public class ScriptModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
