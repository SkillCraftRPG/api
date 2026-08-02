using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Caching;
using SkillCraft.Api.Core.Identity;

namespace SkillCraft.Api.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyDictionary<Guid, Actor>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  public static void Register(IServiceCollection services)
  {
    services.AddSingleton<IActorService, ActorService>();
  }

  private readonly ICacheService _cacheService;
  private readonly IUserGateway _userGateway;

  public ActorService(ICacheService cacheService, IUserGateway userGateway)
  {
    _cacheService = cacheService;
    _userGateway = userGateway;
  }

  public async Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);

    if (capacity > 0)
    {
      HashSet<Guid> userIds = new(capacity);
      foreach (ActorId id in ids)
      {
        Actor? actor = _cacheService.GetActor(id);
        if (actor is null)
        {
          actor = id.GetActor();
          if (actor.Type == ActorType.User)
          {
            userIds.Add(actor.Id);
          }
        }
        else
        {
          actors[id] = actor;
        }
      }

      if (userIds.Count > 0)
      {
        IReadOnlyCollection<User> users = await _userGateway.FindAsync(userIds, cancellationToken);
        foreach (User user in users)
        {
          Actor actor = new(user);
          ActorId id = actor.GetActorId();
          actors[id] = actor;
        }
      }

      foreach (Actor actor in actors.Values)
      {
        _cacheService.SetActor(actor);
      }
    }

    return actors.AsReadOnly();
  }

  public async Task<IReadOnlyDictionary<Guid, Actor>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<Guid, User> foundUsers = new(capacity);

    if (capacity > 0)
    {
      HashSet<Guid> missingIds = new(capacity);
      foreach (Guid id in ids)
      {
        User? user = _cacheService.GetUser(id);
        if (user is null)
        {
          missingIds.Add(id);
        }
        else
        {
          foundUsers[id] = user;
        }
      }

      if (missingIds.Count > 0)
      {
        IReadOnlyCollection<User> users = await _userGateway.FindAsync(missingIds, cancellationToken);
        foreach (User user in users)
        {
          foundUsers[user.Id] = user;
        }
      }

      foreach (User user in foundUsers.Values)
      {
        _cacheService.SetUser(user);
      }
    }

    return foundUsers.ToDictionary(x => x.Key, x => new Actor(x.Value)).AsReadOnly();
  }
}
