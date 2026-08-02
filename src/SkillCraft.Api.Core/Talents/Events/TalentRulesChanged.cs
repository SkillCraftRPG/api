using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentRulesChanged(bool AllowMultiplePurchases, Skill? Skill) : DomainEvent;
