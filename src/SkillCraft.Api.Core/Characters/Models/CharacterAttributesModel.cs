namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterAttributesModel
{
  public CharacterAttributeModel Dexterity { get; set; } = new();
  public CharacterAttributeModel Health { get; set; } = new();
  public CharacterAttributeModel Intellect { get; set; } = new();
  public CharacterAttributeModel Senses { get; set; } = new();
  public CharacterAttributeModel Vigor { get; set; } = new();

  public int PointsSpent => Dexterity.Progression + Health.Progression + Intellect.Progression + Senses.Progression + Vigor.Progression;
}
