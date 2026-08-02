using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemCreated(Name Name) : DomainEvent;
