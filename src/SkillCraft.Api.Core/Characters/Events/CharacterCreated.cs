using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCreated(LineageId LineageId, IReadOnlyCollection<LanguageId> LanguageIds) : DomainEvent;
