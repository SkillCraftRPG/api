using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Customizations.Events;

public record CustomizationDeleted : DomainEvent, IDeleteEvent;
