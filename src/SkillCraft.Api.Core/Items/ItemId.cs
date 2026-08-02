using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items;

public readonly struct ItemId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public ItemId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public ItemId(string value) : this(new StreamId(value))
  {
  }

  public ItemId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Item.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static ItemId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);
  public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ItemId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
