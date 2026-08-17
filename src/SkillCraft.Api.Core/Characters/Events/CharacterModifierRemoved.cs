using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterModifierRemoved(Guid ModifierId) : DomainEvent;
