using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemRenamed(Name Name) : DomainEvent;
