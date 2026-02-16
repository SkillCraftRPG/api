using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageCreated(Name Name) : DomainEvent;
