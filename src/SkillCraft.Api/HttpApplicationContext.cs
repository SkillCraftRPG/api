using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
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

  public UserId UserId
  {
    get
    {
      User user = Context.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
      Actor actor = new(user);
      ActorId actorId = ActorHelper.GetActorId(actor);
      return new UserId(actorId);
    }
  }

  public WorldId WorldId
  {
    get
    {
      WorldModel world = Context.GetWorld() ?? throw new InvalidOperationException("A world is required.");
      return new WorldId(world.Id);
    }
  }
}
