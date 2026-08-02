using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds.Events;

public record WorldDeleted : DomainEvent, IDeleteEvent;
