using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentRequirementsChanged(TalentId? TalentId) : DomainEvent;
