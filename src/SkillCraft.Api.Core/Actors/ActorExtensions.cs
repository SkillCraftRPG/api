using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Actors;

public static class ActorExtensions
{
  private const string RealmKind = "Realm";
  private const char Separator = '|';

  public static Actor GetActor(this ActorId id)
  {
    string[] values = id.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{id}' is not a valid actor identifier.", nameof(id));
    }

    Resource? realm = values.Length == 2 ? Resource.Parse(values.First(), RealmKind) : null;

    Resource actor = Resource.Parse(values.Last());
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
    Resource? realm = actor.RealmId.HasValue ? new Resource(RealmKind, actor.RealmId.Value) : null;
    Resource resource = new(actor.Type.ToString(), actor.Id);
    string value = realm is null ? resource.ToString() : string.Join(Separator, realm, resource);
    return new ActorId(value);
  }
}
