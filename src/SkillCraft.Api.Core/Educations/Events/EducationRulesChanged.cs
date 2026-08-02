using Logitar.EventSourcing;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations.Events;

public record EducationRulesChanged(Skill? Skill, WealthMultiplier? WealthMultiplier, Feature? Feature) : DomainEvent;
