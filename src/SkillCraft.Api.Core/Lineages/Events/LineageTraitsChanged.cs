using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageTraitsChanged(LineageSize Size, LineageWeight Weight, LineageAge Age) : DomainEvent;
