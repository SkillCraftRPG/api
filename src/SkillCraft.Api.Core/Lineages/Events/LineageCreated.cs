using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageCreated(LineageId? ParentId, Name Name) : DomainEvent;
