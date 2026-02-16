using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }
}
