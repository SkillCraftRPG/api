namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageLanguagesPayload
{
  public List<Guid> Ids { get; set; } = [];
  public int Extra { get; set; }
  public string? HtmlContent { get; set; }
}
