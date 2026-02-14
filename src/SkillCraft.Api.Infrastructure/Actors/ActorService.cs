using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using SkillCraft.Api.Infrastructure.Caching;

namespace SkillCraft.Api.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public async Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken)
  {
    int capacity = actorIds.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);
    HashSet<Guid> userIds = new(capacity);

    foreach (ActorId actorId in actorIds)
    {
      Actor? actor = _cacheService.GetActor(actorId);
      if (actor is null)
      {
        actor = ActorHelper.GetActor(actorId);
        if (actor.RealmId.HasValue && actor.Type == ActorType.User)
        {
          userIds.Add(actor.Id);
        }
      }
      else
      {
        actors[actorId] = actor;
      }
    }

    if (userIds.Count > 0)
    {
      //IReadOnlyCollection<User> users = await _userService.FindAsync(userIds, cancellationToken);
      //foreach (User user in users)
      //{
      //  Actor actor = new(user);
      //  ActorId actorId = ActorHelper.GetActorId(actor);
      //  actors[actorId] = actor;
      //} // TODO(fpion): load from Krakenar
    }

    foreach (Actor actor in actors.Values)
    {
      _cacheService.SetActor(actor);
    }

    return actors.AsReadOnly();
  }
}
