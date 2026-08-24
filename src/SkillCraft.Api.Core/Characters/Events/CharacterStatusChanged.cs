using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterStatusChanged(CharacterVitality Vitality, int Stamina, CharacterHope Hope, int BloodAlcoholContent, int Intoxication) : DomainEvent;
