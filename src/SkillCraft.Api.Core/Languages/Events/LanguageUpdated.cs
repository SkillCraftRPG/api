using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<TypicalSpeakers>? TypicalSpeakers { get; set; }
}
