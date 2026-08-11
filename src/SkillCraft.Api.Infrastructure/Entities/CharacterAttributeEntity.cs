namespace SkillCraft.Api.Infrastructure.Entities;

internal record CharacterAttributeEntity
{
  public int Starting { get; set; }
  public int Progression { get; set; }

  public CharacterAttributeEntity(int starting = 0, int progression = 0)
  {
    Starting = starting;
    Progression = progression;
  }
}
