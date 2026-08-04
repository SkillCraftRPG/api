using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCreated(
  Name Name,
  LineageId LineageId,
  IReadOnlyCollection<CustomizationId> CustomizationIds,
  IReadOnlyCollection<LanguageId> LanguageIds) : DomainEvent;
