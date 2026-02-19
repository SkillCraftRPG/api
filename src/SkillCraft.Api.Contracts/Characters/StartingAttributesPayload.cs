namespace SkillCraft.Api.Contracts.Characters;

public record StartingAttributesPayload : IStartingAttributes
{
  public int Dexterity { get; set; }
  public int Health { get; set; }
  public int Intellect { get; set; }
  public int Senses { get; set; }
  public int Vigor { get; set; }
}
