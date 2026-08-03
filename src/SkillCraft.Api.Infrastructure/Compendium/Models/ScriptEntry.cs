using Krakenar.Contracts;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class ScriptEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<LanguageEntry> Languages { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
