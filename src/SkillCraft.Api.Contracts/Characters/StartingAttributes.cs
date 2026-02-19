namespace SkillCraft.Api.Contracts.Characters;

public interface IStartingAttributes
{
  int Dexterity { get; }
  int Health { get; }
  int Intellect { get; }
  int Senses { get; }
  int Vigor { get; }
}
