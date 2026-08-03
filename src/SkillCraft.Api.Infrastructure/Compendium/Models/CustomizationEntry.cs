using Krakenar.Contracts;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class CustomizationEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public CustomizationKind Kind { get; set; }

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
