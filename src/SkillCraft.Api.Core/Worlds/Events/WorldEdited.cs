using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldEdited(Content? Content) : DomainEvent;
