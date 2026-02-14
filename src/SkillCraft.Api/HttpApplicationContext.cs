using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using SkillCraft.Api.Constants;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api;

internal class HttpApplicationContext : IContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public UserId UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");
  public WorldId WorldId => TryGetWorldId() ?? throw new InvalidOperationException("A world is required.");

  public bool IsAdministrator
  {
    get
    {
      User? user = Context.GetUser();
      return user is not null && user.Roles.Any(role => role.UniqueName.Equals(Roles.Administrator, StringComparison.InvariantCultureIgnoreCase));
    }
  }
  public bool IsWorldOwner
  {
    get
    {
      User? user = Context.GetUser();
      WorldModel? world = Context.GetWorld();
      if (user is null || world is null)
      {
        return false;
      }
      return world.Owner.RealmId == user.Realm?.Id && world.Owner.Id == user.Id;
    }
  }

  public UserId? TryGetUserId()
  {
    User? user = Context.GetUser();
    if (user is null)
    {
      return null;
    }
    Actor actor = new(user);
    ActorId actorId = ActorHelper.GetActorId(actor);
    return new UserId(actorId);
  }
  public WorldId? TryGetWorldId()
  {
    WorldModel? world = Context.GetWorld();
    return world is null ? null : new WorldId(world.Id);
  }
}
