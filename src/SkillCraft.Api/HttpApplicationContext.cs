using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api;

internal class HttpApplicationContext : IContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user is not null)
      {
        return new Actor(user).GetActorId();
      }

      ApiKey? apiKey = Context.GetApiKey();
      if (apiKey is not null)
      {
        return new Actor(apiKey).GetActorId();
      }

      return null;
    }
  }
  public Guid UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");
  public WorldId WorldId => TryGetWorldId() ?? throw new InvalidOperationException("A world is required.");
  public Guid WorldUid => TryGetWorldUid() ?? throw new InvalidOperationException("A world is required.");

  public IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes() => Context.GetSessionCustomAttributes();

  public bool IsWorldOwner()
  {
    User? user = Context.GetUser();
    WorldModel? world = Context.GetWorld();
    return user is not null && world is not null && world.Owner.Equals(new Actor(user));
  }

  public Guid? TryGetSessionId() => Context.GetSession()?.Id;
  public Guid? TryGetUserId() => Context.GetUser()?.Id;
  public WorldId? TryGetWorldId()
  {
    WorldModel? world = Context.GetWorld();
    return world is null ? null : new WorldId(world.Id);
  }
  public Guid? TryGetWorldUid() => Context.GetWorld()?.Id;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
  {
    GameContext database = Context.RequestServices.GetRequiredService<GameContext>();
    return await database.SaveChangesAsync(cancellationToken);
  }
}
