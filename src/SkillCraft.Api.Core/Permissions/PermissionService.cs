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
  private readonly IWorldRepository _worldRepository;

  public PermissionService(IContext context, PermissionSettings settings, IWorldRepository worldRepository)
  {
    _context = context;
    _settings = settings;
    _worldRepository = worldRepository;
  }

  public async Task CheckAsync(string action, CancellationToken cancellationToken)
  {
    await CheckAsync(action, resource: null, cancellationToken);
  }
  public async Task CheckAsync(string action, IResource? resource, CancellationToken cancellationToken)
  {
    bool isAllowed = false;

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
      throw new PermissionDeniedException(_context.TryGetUserId(), action, identifier);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    switch (action)
    {
      case Actions.CreateWorld:
        int worlds = await _worldRepository.CountAsync(cancellationToken);
        return worlds < _settings.WorldLimit;
      default:
        return false;
    }
  }

  private bool IsAllowed(string action, World world)
  {
    switch (action)
    {
      case Actions.CreateCustomization:
      case Actions.Update:
        return _context.IsWorldOwner();
      default:
        return false;
    }
  }

  private bool IsAllowed(string action, ResourceIdentifier resource)
  {
    switch (action)
    {
      case Actions.Update:
        return _context.IsWorldOwner() && resource.WorldId == _context.TryGetWorldId();
      default:
        return false;
    }
  }
}
