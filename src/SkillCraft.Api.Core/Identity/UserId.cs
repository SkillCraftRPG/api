using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Actors;

namespace SkillCraft.Api.Core.Identity;

public readonly struct UserId
{
  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public Guid? RealmId { get; }
  public Guid ResourceId { get; }

  public UserId(ActorId actorId)
  {
    ActorId = actorId;

    string[] values = actorId.Value.Split(ActorExtensions.Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{actorId}' is not a valid user identifier.", nameof(actorId));
    }
    else if (values.Length == 2)
    {
      ResourceIdentifier realm = ResourceIdentifier.Parse(values.First(), ActorExtensions.RealmKind);
      RealmId = realm.Id;
    }

    ResourceIdentifier resource = ResourceIdentifier.Parse(values.Last(), ActorType.User.ToString());
    ResourceId = resource.Id;
  }

  public UserId(string value) : this(new ActorId(value))
  {
  }

  public UserId(Guid resourceId, Guid? realmId = null)
  {
    ResourceIdentifier? realm = realmId.HasValue ? new(ActorExtensions.RealmKind, realmId.Value) : null;
    ResourceIdentifier resource = new(ActorType.User.ToString(), resourceId);
    string value = realm is null ? resource.ToString() : string.Join(ActorExtensions.Separator, realm, resource);
    ActorId = new ActorId(value);

    RealmId = realmId;
    ResourceId = resourceId;
  }

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
