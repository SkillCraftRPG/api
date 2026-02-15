using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldCreated(UserId OwnerId, Name Name) : DomainEvent;
