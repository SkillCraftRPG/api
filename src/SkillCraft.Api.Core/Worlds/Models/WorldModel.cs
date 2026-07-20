using Krakenar.Contracts;
using Krakenar.Contracts.Actors;

namespace SkillCraft.Api.Core.Worlds.Models;

public class WorldModel : Aggregate
{
  public Actor Owner { get; set; } = new();

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? HtmlContent { get; set; }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
