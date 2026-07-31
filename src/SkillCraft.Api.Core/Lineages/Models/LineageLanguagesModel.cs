using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageLanguagesModel
{
  public List<LanguageModel> Granted { get; set; } = [];
  public int Extra { get; set; }
  public string? Content { get; set; }
}
