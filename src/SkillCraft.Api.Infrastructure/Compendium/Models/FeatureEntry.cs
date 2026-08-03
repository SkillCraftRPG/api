using Krakenar.Contracts;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class FeatureEntry : Aggregate
{
  public string Key { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public string? HtmlContent { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
