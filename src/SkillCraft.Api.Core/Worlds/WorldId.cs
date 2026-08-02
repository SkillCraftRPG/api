using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Worlds;

public readonly struct WorldId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public Guid ResourceId { get; }

  public WorldId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value, World.ResourceKind);
    ResourceId = resource.Id;
  }

  public WorldId(string value) : this(new StreamId(value))
  {
  }

  public WorldId(Guid resourceId)
  {
    ResourceIdentifier resource = new(World.ResourceKind, resourceId);
    StreamId = new StreamId(resource.ToString());

    ResourceId = resourceId;
  }

  public static WorldId NewId() => new(Guid.NewGuid());

  public static bool operator ==(WorldId left, WorldId right) => left.Equals(right);
  public static bool operator !=(WorldId left, WorldId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is WorldId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
