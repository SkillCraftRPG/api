using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public readonly struct TalentId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public TalentId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Talent.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public TalentId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Talent.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public TalentId(string value) : this(new StreamId(value))
  {
  }

  public static TalentId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(TalentId left, TalentId right) => left.Equals(right);
  public static bool operator !=(TalentId left, TalentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TalentId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
