using Krakenar.Contracts.Actors;
using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterLanguageModel
{
  public LanguageModel Language { get; set; } = new();

  public CharacterLanguageSource Source { get; set; }
  public string? Target { get; set; }
  public string? Notes { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
