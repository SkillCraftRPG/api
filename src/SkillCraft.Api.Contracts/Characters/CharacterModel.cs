using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Characters;

public class CharacterModel : Aggregate
{
  public string Name { get; set; }

  // TODO(fpion): complete this

  public CharacterModel() : this(string.Empty)
  {
  }

  public CharacterModel(string name)
  {
    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
