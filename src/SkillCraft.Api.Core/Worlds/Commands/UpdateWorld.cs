using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds.Events;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : ICommand<WorldModel?>;

internal class UpdateWorldCommandHandler : ICommandHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(IContext context, IPermissionService permissionService, IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<WorldModel?> HandleAsync(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    payload.Validate();

    World? world = await _worldRepository.LoadAsync(command.Id, cancellationToken);
    if (world is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, world, cancellationToken);

    WorldSnapshot snapshot = new(world);

    if (!string.IsNullOrWhiteSpace(payload.Key))
    {
      world.Key = SlugHelper.Format(payload.Key);
    }
    if (payload.Name is not null)
    {
      world.Name = payload.Name.Value?.CleanTrim();
    }
    if (payload.HtmlContent is not null)
    {
      world.HtmlContent = payload.HtmlContent.Value?.CleanTrim();
    }

    WorldUpdated? record = snapshot.Compare(world);
    if (record is not null)
    {
      world.Update(_context.UserId);
      _worldRepository.Update(world, record);

      await _worldRepository.EnsureUnicityAsync(world, cancellationToken);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _worldRepository.ReadAsync(world, cancellationToken);
  }
}
