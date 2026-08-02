using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldKeyChanged(Key Key) : DomainEvent;
