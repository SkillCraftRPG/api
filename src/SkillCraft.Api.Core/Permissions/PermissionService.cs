using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Permissions;

public interface IPermissionService
{
  Task CheckAsync(string action, CancellationToken cancellationToken = default);
  Task CheckAsync(string action, IEntityProvider? resource, CancellationToken cancellationToken = default);
}

internal class PermissionService : IPermissionService
{
  private readonly IContext _context;
  private readonly PermissionSettings _settings;
  private readonly IWorldQuerier _worldQuerier;

  public PermissionService(IContext context, PermissionSettings settings, IWorldQuerier worldQuerier)
  {
    _context = context;
    _settings = settings;
    _worldQuerier = worldQuerier;
  }

  public async Task CheckAsync(string action, CancellationToken cancellationToken)
  {
    await CheckAsync(action, resource: null, cancellationToken);
  }
  public async Task CheckAsync(string action, IEntityProvider? resource, CancellationToken cancellationToken)
  {
    if (_context.IsAdministrator)
    {
      return;
    }

    Entity? entity = resource?.GetEntity();
    bool isAllowed = (entity?.Kind) switch
    {
      null => await IsAllowedAsync(action, cancellationToken),
      Customization.EntityKind or World.EntityKind => await IsAllowedAsync(action, entity, cancellationToken),
      _ => throw new NotSupportedException($"The entity kind '{entity.Kind}' is not supported."),
    };

    if (!isAllowed)
    {
      UserId? userId = _context.TryGetUserId();
      WorldId? worldId = _context.TryGetWorldId();
      throw new PermissionDeniedException(userId, action, entity, worldId);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken) => action switch
  {
    Actions.CreateCustomization => _context.IsWorldOwner,
    Actions.CreateWorld => await CanCreateWorldAsync(cancellationToken),
    _ => false,
  };
  private async Task<bool> CanCreateWorldAsync(CancellationToken cancellationToken)
  {
    int count = await _worldQuerier.CountAsync(cancellationToken);
    return count < _settings.WorldLimit;
  }

  private async Task<bool> IsAllowedAsync(string action, Entity entity, CancellationToken cancellationToken)
  {
    WorldId? worldId = entity.WorldId;
    if (worldId is null && entity.Kind == World.EntityKind)
    {
      worldId = new(entity.Id);
    }
    return worldId.HasValue && _context.WorldId == worldId.Value && _context.IsWorldOwner;
  }
}
