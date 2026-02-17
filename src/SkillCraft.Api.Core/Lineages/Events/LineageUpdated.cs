using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public LineageLanguages? Languages { get; set; }
  public Names? Names { get; set; }

  public Speeds? Speeds { get; set; }
  public Size? Size { get; set; }
  public Weight? Weight { get; set; }
  public Age? Age { get; set; }
}
