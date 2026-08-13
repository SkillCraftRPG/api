using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public ItemCategory Category { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public int? Price { get; private set; }
  public int? Weight { get; private set; }

  public ItemRarity? Rarity { get; private set; }

  public int? MaximumCharges { get; private set; }
  public DepletionBehavior? ChargesDepletionBehavior { get; private set; }
  public ItemEntity? Replacement { get; private set; }
  public int? ReplacementId { get; private set; }
  public List<ItemEntity> ReplacedItems { get; private set; } = [];

  public string? Properties { get; private set; }

  public ItemEntity(ItemCreated @event) : base(@event)
  {
    ItemId itemId = new(@event.StreamId);
    WorldId = itemId.WorldId.ResourceId;
    Id = itemId.ResourceId;

    Category = @event.Category;

    Name = @event.Name.Value;
  }

  private ItemEntity() : base()
  {
  }

  public void Edit(ItemEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Replacement is not null)
    {
      actorIds.AddRange(Replacement.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Rename(ItemRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRules(ItemEntity? replacement, ItemRulesChanged @event)
  {
    base.Update(@event);

    Price = @event.Price?.Value;
    Weight = @event.Weight?.Value;

    Rarity = @event.Rarity;

    MaximumCharges = @event.Charges?.Maximum;
    ChargesDepletionBehavior = @event.Charges?.DepletionBehavior;
    Replacement = replacement;
    ReplacementId = replacement?.ItemId;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
