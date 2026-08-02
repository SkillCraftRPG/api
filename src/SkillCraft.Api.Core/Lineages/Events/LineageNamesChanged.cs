using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageNamesChanged(LineageNames Names) : DomainEvent;
