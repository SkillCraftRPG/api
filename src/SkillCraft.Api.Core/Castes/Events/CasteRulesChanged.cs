using Logitar.EventSourcing;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteRulesChanged(Skill? Skill, Roll? WealthRoll, Feature? Feature) : DomainEvent;
