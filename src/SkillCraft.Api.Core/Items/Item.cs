using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items;

public class Item : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Item";

  public int ItemId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public double? Price { get; set; }
  public double? Weight { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Item(World world, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Item()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Item item && item.ItemId == ItemId;
  public override int GetHashCode() => ItemId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (ItemId={ItemId})";
}
