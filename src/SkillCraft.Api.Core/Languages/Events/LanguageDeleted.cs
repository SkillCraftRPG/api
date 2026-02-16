using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageDeleted : DomainEvent, IDeleteEvent;
