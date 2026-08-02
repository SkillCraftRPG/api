using Logitar.CQRS;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record CreateOrReplaceWorldCommand(CreateOrReplaceWorldPayload Payload, Guid? Id) : ICommand<CreateOrReplaceWorldResult>;

internal class CreateOrReplaceWorldCommandHandler : ICommandHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceWorldCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceWorldResult> HandleAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldPayload payload = command.Payload;
    payload.Validate();

    World? world = null;
    WorldId worldId = WorldId.NewId();
    if (command.Id.HasValue)
    {
      worldId = new WorldId(command.Id.Value);
      world = await _worldRepository.LoadAsync(worldId, cancellationToken);
    }

    UserId userId = _context.UserId;
    Key key = new(payload.Key);

    bool created = false;
    if (world is null)
    {
      await _permissionService.CheckAsync(Actions.CreateWorld, cancellationToken);

      world = new World(userId, key, worldId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

      world.SetKey(key, userId.ActorId);
    }

    world.Rename(Name.TryCreate(payload.Name), userId.ActorId);
    world.Edit(Content.TryCreate(payload.Content), userId.ActorId);

    await _worldQuerier.EnsureUnicityAsync(world, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, created);
  }
}
