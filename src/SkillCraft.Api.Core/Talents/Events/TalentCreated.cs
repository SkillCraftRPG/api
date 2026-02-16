using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentCreated(Tier Tier, Name Name) : DomainEvent;
