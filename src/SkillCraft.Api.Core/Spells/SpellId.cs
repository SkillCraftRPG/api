using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Spells;

public readonly struct SpellId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public SpellId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public SpellId(string value) : this(new StreamId(value))
  {
  }

  public SpellId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Spell.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static SpellId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(SpellId left, SpellId right) => left.Equals(right);
  public static bool operator !=(SpellId left, SpellId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SpellId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
