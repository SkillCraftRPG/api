using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageRenamed(Name Name) : DomainEvent;
