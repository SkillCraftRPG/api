using Krakenar.Contracts;
using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class SkillEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public Skill Value { get; set; }
  public AttributeEntry? Attribute { get; set; }

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<TalentEntry> Talents { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
