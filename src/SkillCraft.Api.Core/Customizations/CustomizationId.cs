using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations;

public readonly struct CustomizationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public CustomizationId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Customization.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public CustomizationId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Customization.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public CustomizationId(string value) : this(new StreamId(value))
  {
  }

  public static CustomizationId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(CustomizationId left, CustomizationId right) => left.Equals(right);
  public static bool operator !=(CustomizationId left, CustomizationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CustomizationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
