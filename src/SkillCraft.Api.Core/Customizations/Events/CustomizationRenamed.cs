using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Customizations.Events;

public record CustomizationRenamed(Name Name) : DomainEvent;
