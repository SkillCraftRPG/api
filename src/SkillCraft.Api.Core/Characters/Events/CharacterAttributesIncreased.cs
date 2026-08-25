using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterAttributesIncreased(int Dexterity, int Health, int Intellect, int Senses, int Vigor, int Vitality, int Stamina) : DomainEvent;
