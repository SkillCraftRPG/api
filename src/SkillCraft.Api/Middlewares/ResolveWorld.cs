using Microsoft.Extensions.Primitives;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Extensions;

namespace SkillCraft.Api.Middlewares;

internal class ResolveWorld
{
  public const string Header = "World";

  private readonly RequestDelegate _next;

  public ResolveWorld(RequestDelegate next)
  {
    _next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, ILogger<ResolveWorld> logger, IWorldQuerier worldQuerier)
  {
    if (context.Request.Headers.TryGetValue(Header, out StringValues worlds))
    {
      IReadOnlyCollection<string> sanitized = worlds.Sanitize();
      if (sanitized.Count > 1)
      {
        logger.LogWarning("Multiple {Header} header values were received ({Sanitized} sanitized, {Total} total). Ignoring world resolving.",
          Header, sanitized.Count, worlds.Count);
      }
      else if (sanitized.Count == 1)
      {
        WorldModel? world = null;
        if (Guid.TryParse(sanitized.Single(), out Guid id))
        {
          world = await worldQuerier.ReadAsync(id);
        }
        if (world is null)
        {
          throw new EntityNotFoundException(new Entity(World.EntityKind, id), Header);
        }
        context.SetWorld(world);
      }
    }

    await _next(context);
  }
}
