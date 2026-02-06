using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace SkillCraft.Api.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  public async Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken)
  {
    int capacity = actorIds.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);
    HashSet<ActorId> missingIds = new(capacity);

    // TODO(fpion): implement

    return actors.AsReadOnly();
  }
}
