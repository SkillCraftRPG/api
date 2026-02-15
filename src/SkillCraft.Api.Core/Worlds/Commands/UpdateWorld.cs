using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds.Validators;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : ICommand<WorldModel?>;

internal class UpdateWorldCommandHandler : ICommandHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IContext _context;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateWorldCommandHandler(
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

  public async Task<WorldModel?> HandleAsync(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    new UpdateWorldValidator().ValidateAndThrow(payload);

    WorldId worldId = new(command.Id);
    World? world = await _worldRepository.LoadAsync(worldId, cancellationToken);
    if (world is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      world.Name = new Name(payload.Name);
    }
    if (payload.Description is not null)
    {
      world.Description = Description.TryCreate(payload.Description.Value);
    }

    world.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      world,
      async () => await _worldRepository.SaveAsync(world, cancellationToken),
      cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
