using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds.Events;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record CreateOrReplaceWorldCommand(CreateOrReplaceWorldPayload Payload, Guid? Id) : ICommand<CreateOrReplaceWorldResult>;

internal class CreateOrReplaceWorldCommandHandler : ICommandHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceWorldCommandHandler(IContext context, IPermissionService permissionService, IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceWorldResult> HandleAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldPayload payload = command.Payload;
    payload.Validate();

    World? world = null;
    if (command.Id.HasValue)
    {
      world = await _worldRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;

    WorldSnapshot? snapshot = null;
    if (world is null)
    {
      await _permissionService.CheckAsync(Actions.CreateWorld, cancellationToken);

      world = new World(userId, command.Id);
      _worldRepository.Add(world);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

      snapshot = new WorldSnapshot(world);
    }

    world.Key = SlugHelper.Format(payload.Key);
    world.Name = payload.Name?.CleanTrim();
    world.Content = payload.Content?.CleanTrim();

    if (snapshot is not null)
    {
      WorldUpdated? record = snapshot.Compare(world);
      if (record is not null)
      {
        world.Update(userId);
        _worldRepository.Update(world, record);
      }
    }

    await _worldRepository.EnsureUnicityAsync(world, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);

    WorldModel model = await _worldRepository.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, Created: snapshot is null);
  }
}
