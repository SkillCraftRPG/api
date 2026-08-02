using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public double? Price { get; private set; }
  public double? Weight { get; private set; }

  public ItemEntity(Item item) : base(item)
  {
    WorldId = item.WorldId.ResourceId;
    Id = item.ResourceId;

    Update(item);
  }

  public ItemEntity(ItemCreated @event) : base(@event)
  {
    ItemId itemId = new(@event.StreamId);
    WorldId = itemId.WorldId.ResourceId;
    Id = itemId.ResourceId;

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

  public void Rename(ItemRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRules(ItemRulesChanged @event)
  {
    base.Update(@event);

    Price = @event.Price?.Value;
    Weight = @event.Weight?.Value;
  }

  public void Update(Item item)
  {
    base.Update(item);

    Name = item.Name.Value;
    Summary = item.Summary?.Value;
    Content = item.Content?.Value;

    Price = item.Price?.Value;
    Weight = item.Weight?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
