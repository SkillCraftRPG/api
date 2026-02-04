using Logitar.CQRS;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record DeleteWorldCommand(Guid Id) : ICommand<WorldModel?>;

internal class DeleteWorldCommandHandler : ICommandHandler<DeleteWorldCommand, WorldModel?>
{
  private readonly IContext _context;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteWorldCommandHandler(
    IContext context,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<WorldModel?> HandleAsync(DeleteWorldCommand command, CancellationToken cancellationToken)
  {
    WorldId worldId = new(command.Id);
    World? world = await _worldRepository.LoadAsync(worldId, cancellationToken);
    if (world is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, world, cancellationToken);
    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);

    world.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      world,
      async () => await _worldRepository.SaveAsync(world, cancellationToken),
      cancellationToken);

    return model;
  }
}
