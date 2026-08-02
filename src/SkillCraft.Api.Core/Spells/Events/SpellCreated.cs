using Logitar.EventSourcing;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Spells.Events;

public record SpellCreated(TalentTier Tier, Name Name) : DomainEvent;
