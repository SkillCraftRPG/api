using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Languages.Events;

public record LanguageEdited(Summary? Summary, Content? Content) : DomainEvent;
