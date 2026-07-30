namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageNamesModel
{
  public List<string> Family { get; set; } = [];
  public List<string> Female { get; set; } = [];
  public List<string> Male { get; set; } = [];
  public List<string> Unisex { get; set; } = [];
  public List<NameCategory> Custom { get; set; } = [];
  public string? HtmlContent { get; set; }
}
