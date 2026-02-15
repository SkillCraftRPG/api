using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Parties.Events;

public record PartyUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Description>? Description { get; set; }
}
