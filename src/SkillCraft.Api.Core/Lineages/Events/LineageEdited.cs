using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageEdited(Summary? Summary, Content? Content) : DomainEvent;
