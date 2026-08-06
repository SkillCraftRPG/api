using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCreated(
  Name Name,
  DominantHand? DominantHand,
  LineageId LineageId,
  CasteId CasteId,
  EducationId EducationId,
  IReadOnlyCollection<CustomizationId> CustomizationIds,
  IReadOnlyCollection<LanguageId> LanguageIds,
  IReadOnlyCollection<CharacterTalent> Talents) : DomainEvent;
