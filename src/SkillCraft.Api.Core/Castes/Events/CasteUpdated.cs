using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }
}
