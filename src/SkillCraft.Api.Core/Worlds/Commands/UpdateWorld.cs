using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : ICommand<WorldModel?>;

internal class UpdateWorldCommandHandler : ICommandHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(IContext context, IPermissionService permissionService, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<WorldModel?> HandleAsync(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    payload.Validate();

    WorldId worldId = new(command.Id);
    World? world = await _worldRepository.LoadAsync(worldId, cancellationToken);
    if (world is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

    ActorId? actorId = _context.ActorId;

    Key? key = Key.TryCreate(payload.Key);
    if (key is not null)
    {
      world.SetKey(key, actorId);
    }

    if (payload.Name is not null)
    {
      world.Rename(Name.TryCreate(payload.Name.Value), actorId);
    }

    if (payload.Content is not null)
    {
      world.Edit(Content.TryCreate(payload.Content.Value), actorId);
    }

    await _worldQuerier.EnsureUnicityAsync(world, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
