using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Actors;

public static class ActorExtensions
{
  public const string RealmKind = "Realm";
  public const char Separator = '|';

  public static Actor GetActor(this ActorId id)
  {
    string[] values = id.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{id}' is not a valid actor identifier.", nameof(id));
    }

    ResourceIdentifier? realm = values.Length == 2 ? ResourceIdentifier.Parse(values.First(), RealmKind) : null;

    ResourceIdentifier actor = ResourceIdentifier.Parse(values.Last());
    if (!Enum.TryParse(actor.Kind, out ActorType type) || !Enum.IsDefined(type))
    {
      throw new ArgumentException($"The actor type '{actor.Kind}' is not valid.", nameof(id));
    }

    return new Actor
    {
      RealmId = realm?.Id,
      Id = actor.Id,
      Type = type
    };
  }

  public static ActorId GetActorId(this Actor actor)
  {
    ResourceIdentifier? realm = actor.RealmId.HasValue ? new ResourceIdentifier(RealmKind, actor.RealmId.Value) : null;
    ResourceIdentifier resource = new(actor.Type.ToString(), actor.Id);
    string value = realm is null ? resource.ToString() : string.Join(Separator, realm, resource);
    return new ActorId(value);
  }
}
