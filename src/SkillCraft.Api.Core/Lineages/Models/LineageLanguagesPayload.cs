namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageLanguagesPayload
{
  public List<Guid> LanguageIds { get; set; } = []; // TODO(fpion): rename
  public int Extra { get; set; }
  public string? HtmlContent { get; set; }
}
