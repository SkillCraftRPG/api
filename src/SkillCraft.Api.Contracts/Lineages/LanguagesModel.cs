using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Contracts.Lineages;

public record LanguagesModel
{
  public List<LanguageModel> Items { get; set; } = [];
  public int Extra { get; set; }
  public string? Text { get; set; }
}
