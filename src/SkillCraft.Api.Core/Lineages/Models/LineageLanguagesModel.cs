using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageLanguagesModel
{
  public List<LanguageModel> Languages { get; set; } = []; // TODO(fpion): rename
  public int Extra { get; set; }
  public string? HtmlContent { get; set; }
}
