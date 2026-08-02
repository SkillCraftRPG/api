using Logitar.EventSourcing;
using SkillCraft.Api.Core.Identity;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldCreated(UserId OwnerId, Key Key) : DomainEvent;
