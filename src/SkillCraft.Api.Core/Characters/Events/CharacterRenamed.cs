using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterRenamed(Name Name) : DomainEvent;
