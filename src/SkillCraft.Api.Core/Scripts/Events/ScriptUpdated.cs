using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Scripts.Events;

public record ScriptUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }
}
