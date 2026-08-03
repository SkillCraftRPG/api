using Krakenar.Contracts;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class AttributeEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public GameAttribute Value { get; set; }
  public AttributeCategory? Category { get; set; }

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<StatisticEntry> Statistics { get; set; } = [];
  public List<SkillEntry> Skills { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
