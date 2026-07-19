using Logitar.CQRS;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : ICommand<WorldModel?>;

internal class UpdateWorldCommandHandler : ICommandHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IContext _context;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(IContext context, IWorldRepository worldRepository)
  {
    _context = context;
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

    world.Update(
      string.IsNullOrWhiteSpace(payload.Key) ? world.Key : payload.Key,
      payload.Name is null ? world.Name : payload.Name.Value,
      payload.Description is null ? world.Description : payload.Description.Value,
      _context.UserId);

    await _worldRepository.EnsureUnicityAsync(world, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);

    return await _worldRepository.ReadAsync(world, cancellationToken);
  }
}
