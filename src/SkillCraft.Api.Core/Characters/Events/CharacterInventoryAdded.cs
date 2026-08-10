using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterInventoryAdded(ItemId ItemId, int Quantity) : DomainEvent;
