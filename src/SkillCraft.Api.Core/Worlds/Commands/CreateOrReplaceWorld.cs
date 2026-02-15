using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds.Validators;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record CreateOrReplaceWorldCommand(CreateOrReplaceWorldPayload Payload, Guid? Id) : ICommand<CreateOrReplaceWorldResult>;

internal class CreateOrReplaceWorldCommandHandler : ICommandHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IContext _context;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplaceWorldCommandHandler(
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

  public async Task<CreateOrReplaceWorldResult> HandleAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldPayload payload = command.Payload;
    new CreateOrReplaceWorldValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;

    WorldId worldId = WorldId.NewId();
    World? world = null;
    if (command.Id.HasValue)
    {
      worldId = new(command.Id.Value);
      world = await _worldRepository.LoadAsync(worldId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (world is null)
    {
      await _permissionService.CheckAsync(Actions.CreateWorld, cancellationToken);

      world = new World(userId, name, worldId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

      world.Name = name;
    }

    world.Description = Description.TryCreate(payload.Description);

    world.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      world,
      async () => await _worldRepository.SaveAsync(world, cancellationToken),
      cancellationToken);

    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, created);
  }
}
