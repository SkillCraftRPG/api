using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Spells.Events;

public record SpellEdited(Summary? Summary, Content? Content) : DomainEvent;
