using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCreated(
  LineageId LineageId,
  IReadOnlyCollection<LanguageId> LanguageIds,
  Name Name,
  DominantHand? DominantHand,
  IReadOnlyCollection<CustomizationId> CustomizationIds,
  CasteId CasteId,
  EducationId EducationId,
  IReadOnlyDictionary<Guid, CharacterTalent> Talents,
  StartingAttributes Attributes,
  IReadOnlyDictionary<Skill, int> Skills,
  CharacterAppearance Appearance,
  Alignment? Alignment,
  CharacterPersonality Personality,
  Background? Background) : DomainEvent;
