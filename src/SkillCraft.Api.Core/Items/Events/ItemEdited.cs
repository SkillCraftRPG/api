using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemEdited(Summary? Summary, Content? Content) : DomainEvent;
