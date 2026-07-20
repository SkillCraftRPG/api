namespace SkillCraft.Api.Core.Scripts.Events;

public class ScriptUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Description { get; set; }

  public ScriptUpdated() : base()
  {
  }

  public ScriptUpdated(Script script) : base(script)
  {
  }
}
