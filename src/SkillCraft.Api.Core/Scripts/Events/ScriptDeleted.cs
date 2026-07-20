namespace SkillCraft.Api.Core.Scripts.Events;

public class ScriptDeleted : DeleteEvent
{
  public ScriptDeleted() : base()
  {
  }

  public ScriptDeleted(Script script, Guid userId) : base(script, userId)
  {
  }
}
