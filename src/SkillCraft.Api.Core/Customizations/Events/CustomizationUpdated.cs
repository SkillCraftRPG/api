using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Customizations.Events;

public record CustomizationUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }
}
