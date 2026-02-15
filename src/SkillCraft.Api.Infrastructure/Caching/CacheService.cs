using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);
}

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memory;
  private readonly CacheSettings _settings;

  public CacheService(IMemoryCache memory, CacheSettings settings)
  {
    _memory = memory;
    _settings = settings;
  }

  public Actor? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return _memory.TryGetValue(key, out object? value) ? (Actor?)value : null;
  }
  public void RemoveActor(ActorId id)
  {
    string key = GetActorKey(id);
    _memory.Remove(key);
  }
  public void SetActor(Actor actor)
  {
    ActorId id = ActorHelper.GetActorId(actor);
    string key = GetActorKey(id);
    _memory.Set(key, actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id={id}";
}
