using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterDeleted : DomainEvent, IDeleteEvent;
