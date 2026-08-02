using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentCreated(TalentTier Tier, Name Name) : DomainEvent;
