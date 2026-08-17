using Krakenar.Contracts.Actors;

namespace SkillCraft.Api.Core.Characters.Models;

public class CharacterModifierModel
{
  public Guid Id { get; set; }

  public CharacterModifierKind Kind { get; set; }
  public string Target { get; set; } = string.Empty;

  public int Value { get; set; }

  public string? Name { get; set; }
  public string? Notes { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
