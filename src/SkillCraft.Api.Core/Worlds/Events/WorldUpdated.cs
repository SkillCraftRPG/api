using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Description>? Description { get; set; }
}
