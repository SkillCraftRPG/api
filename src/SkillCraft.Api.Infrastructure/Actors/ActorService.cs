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
    HashSet<ActorId> missingIds = new(capacity);

    foreach (ActorId actorId in actorIds)
    {
      Actor? actor = _cacheService.GetActor(actorId);
      if (actor is null)
      {
        missingIds.Add(actorId);
      }
      else
      {
        actors[actorId] = actor;
      }
    }

    // TODO(fpion): fetch from Krakenar

    foreach (Actor actor in actors.Values)
    {
      _cacheService.SetActor(actor);
    }

    return actors.AsReadOnly();
  }
}
