using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Permissions;

public interface IPermissionService
{
  Task CheckAsync(string action, CancellationToken cancellationToken = default);
  Task CheckAsync(string action, IResource? resource, CancellationToken cancellationToken = default);
}

internal class PermissionService : IPermissionService
{
  public static void Register(IServiceCollection services)
  {
    services.AddSingleton(serviceProvider => PermissionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddTransient<IPermissionService, PermissionService>();
  }

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
  public async Task CheckAsync(string action, IResource? resource, CancellationToken cancellationToken)
  {
    bool isAllowed;

    ResourceIdentifier? identifier = null;
    if (resource is null)
    {
      isAllowed = await IsAllowedAsync(action, cancellationToken);
    }
    else
    {
      identifier = resource.Identifier;
      isAllowed = resource is World world ? IsAllowed(action, world) : IsAllowed(action, identifier);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(_context.ActorId, action, identifier, _context.TryGetWorldId());
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    switch (action)
    {
      case Actions.CreateCaste:
      case Actions.CreateCustomization:
      case Actions.CreateEducation:
      case Actions.CreateItem:
      case Actions.CreateLanguage:
      case Actions.CreateLineage:
      case Actions.CreateScript:
      case Actions.CreateSpell:
      case Actions.CreateTalent:
        return _context.IsWorldOwner();
      case Actions.CreateWorld:
        int worlds = await _worldQuerier.CountAsync(cancellationToken);
        return worlds < _settings.WorldLimit;
      default:
        return false;
    }
  }

  private bool IsAllowed(string action, World world)
  {
    return action == Actions.Update && world.OwnerId == _context.TryGetUserId();
  }

  private bool IsAllowed(string action, ResourceIdentifier resource)
  {
    return action == Actions.Update && _context.IsWorldOwner() && resource.WorldId == _context.TryGetWorldId();
  }
}
