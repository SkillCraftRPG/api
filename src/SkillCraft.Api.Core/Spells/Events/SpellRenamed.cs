using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Spells.Events;

public record SpellRenamed(Name Name) : DomainEvent;
