using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentRenamed(Name Name) : DomainEvent;
