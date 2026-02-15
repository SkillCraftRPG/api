using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Actors;

public static class ActorHelper
{
  private const string Realm = "Realm";
  private const char Separator = '|';

  public static Actor GetActor(ActorId id)
  {
    Actor actor = new();

    string[] parts = id.Value.Split(Separator);
    if (parts.Length > 2)
    {
      throw new ArgumentException($"The value '{id}' is not a valid actor identifier.", nameof(id));
    }
    else if (parts.Length == 2)
    {
      Entity realm = Entity.Parse(parts.First());
      if (realm.Kind != Realm)
      {
        throw new ArgumentException($"The scoping entity '{realm}' should be a {Realm}.", nameof(id));
      }
      actor.RealmId = realm.Id;
    }

    Entity entity = Entity.Parse(parts.Last());
    actor.Type = Enum.Parse<ActorType>(entity.Kind);
    actor.Id = entity.Id;

    return actor;
  }

  public static ActorId GetActorId(Actor actor)
  {
    List<string> values = new(capacity: 2);

    if (actor.RealmId.HasValue)
    {
      Entity realm = new(Realm, actor.RealmId.Value);
      values.Add(realm.ToString());
    }

    Entity entity = new(actor.Type.ToString(), actor.Id);
    values.Add(entity.ToString());

    return new ActorId(string.Join(Separator, values));
  }
}
