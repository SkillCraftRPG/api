using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCreated(
  Name Name,
  Characteristics Characteristics,
  LineageId LineageId,
  CasteId CasteId,
  EducationId EducationId,
  IReadOnlyCollection<LanguageId> LanguageIds,
  IReadOnlyCollection<CustomizationId> CustomizationIds) : DomainEvent;
