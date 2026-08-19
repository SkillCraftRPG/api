using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterLanguageChanged(LanguageId LanguageId, CharacterLanguageAcquisition Acquisition) : DomainEvent;
