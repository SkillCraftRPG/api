using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemRulesChanged(Price? Price, Weight? Weight, ItemRarity? Rarity, ItemCharges? Charges) : DomainEvent;
