using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Talents;
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
    bool isAllowed;
    Entity? entity = resource?.GetEntity();
    if (_context.IsAdministrator)
    {
      isAllowed = true;
    }
    else if (resource is World world)
    {
      isAllowed = IsAllowed(action, world);
    }
    else
    {
      isAllowed = (entity?.Kind) switch
      {
        null => await IsAllowedAsync(action, cancellationToken),
        Caste.EntityKind or Customization.EntityKind or Education.EntityKind or Language.EntityKind or Party.EntityKind or Script.EntityKind or Talent.EntityKind => IsAllowed(action, entity),
        _ => throw new NotSupportedException($"The entity kind '{entity.Kind}' is not supported."),
      };
    }

    if (!isAllowed)
    {
      UserId? userId = _context.TryGetUserId();
      WorldId? worldId = _context.TryGetWorldId();
      throw new PermissionDeniedException(userId, action, entity, worldId);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken) => action switch
  {
    Actions.CreateCaste or Actions.CreateCustomization or Actions.CreateEducation or Actions.CreateLanguage or Actions.CreateParty or Actions.CreateScript or Actions.CreateTalent => _context.IsWorldOwner,
    Actions.CreateWorld => await CanCreateWorldAsync(cancellationToken),
    _ => false,
  };
  private async Task<bool> CanCreateWorldAsync(CancellationToken cancellationToken)
  {
    int count = await _worldQuerier.CountAsync(cancellationToken);
    return count < _settings.WorldLimit;
  }

  private bool IsAllowed(string action, World world) => action switch
  {
    Actions.Delete or Actions.Update => world.OwnerId == _context.UserId,
    _ => false,
  };

  private bool IsAllowed(string action, Entity entity) => action switch
  {
    Actions.Delete or Actions.Update => entity.WorldId.HasValue && entity.WorldId.Value == _context.WorldId && _context.IsWorldOwner,
    _ => false,
  };
}
