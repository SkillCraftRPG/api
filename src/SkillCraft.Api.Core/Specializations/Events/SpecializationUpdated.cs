using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Specializations.Events;

public record SpecializationUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public Requirements? Requirements { get; set; }
  public Options? Options { get; set; }
  public Change<Doctrine>? Doctrine { get; set; }
}
