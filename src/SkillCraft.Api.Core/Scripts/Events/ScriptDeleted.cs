using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Scripts.Events;

public record ScriptDeleted : DomainEvent, IDeleteEvent;
