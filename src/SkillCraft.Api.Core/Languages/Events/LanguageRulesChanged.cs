using Logitar.EventSourcing;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageRulesChanged(TypicalSpeakers? TypicalSpeakers, ScriptId? ScriptId) : DomainEvent;
