using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public readonly struct TalentId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public TalentId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public TalentId(string value) : this(new StreamId(value))
  {
  }

  public TalentId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Talent.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static TalentId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(TalentId left, TalentId right) => left.Equals(right);
  public static bool operator !=(TalentId left, TalentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TalentId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
