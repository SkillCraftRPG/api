namespace SkillCraft.Api.Core.Talents.Events;

public class TalentDeleted : DeleteEvent
{
  public TalentDeleted() : base()
  {
  }

  public TalentDeleted(Talent talent, Guid userId) : base(talent, userId)
  {
  }
}
