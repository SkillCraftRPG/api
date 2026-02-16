using Logitar.EventSourcing;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<ScriptId?>? ScriptId { get; set; }
  public Change<TypicalSpeakers>? TypicalSpeakers { get; set; }
}
