using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterLanguageModel
{
  public LanguageModel Language { get; set; } = new();

  // TODO(fpion): Source, Target and Notes
}
