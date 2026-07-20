namespace SkillCraft.Api.Core.Scripts.Events;

public class ScriptCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public ScriptCreated() : base()
  {
  }

  public ScriptCreated(Script script) : base(script)
  {
    Name = script.Name;
    Description = script.Description;
  }
}
