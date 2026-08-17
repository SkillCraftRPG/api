using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterModifierChanged(Guid ModifierId, CharacterModifier Modifier) : DomainEvent;
