using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemCreated(ItemCategory Category, Name Name) : DomainEvent;
