using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldRenamed(Name? Name) : DomainEvent;
