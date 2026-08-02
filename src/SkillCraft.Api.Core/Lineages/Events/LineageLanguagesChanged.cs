using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageLanguagesChanged(LineageLanguages Languages) : DomainEvent;
