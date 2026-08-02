using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations;

public readonly struct CustomizationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public CustomizationId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public CustomizationId(string value) : this(new StreamId(value))
  {
  }

  public CustomizationId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Customization.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static CustomizationId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(CustomizationId left, CustomizationId right) => left.Equals(right);
  public static bool operator !=(CustomizationId left, CustomizationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CustomizationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
