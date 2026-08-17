using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterProfileChanged(
  DominantHand? DominantHand,
  CharacterAppearance Appearance,
  Alignment? Alignment,
  CharacterPersonality Personality,
  Background? Background) : DomainEvent;
