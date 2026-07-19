using Logitar.CQRS;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds.Commands;

internal record CreateOrReplaceWorldCommand(CreateOrReplaceWorldPayload Payload, Guid? Id) : ICommand<CreateOrReplaceWorldResult>;

internal class CreateOrReplaceWorldCommandHandler : ICommandHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IContext _context;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceWorldCommandHandler(IContext context, IWorldRepository worldRepository)
  {
    _context = context;
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

    bool created = false;
    if (world is null)
    {
      world = new World(_context.UserId, payload.Key, command.Id, payload.Name, payload.Description);
      _worldRepository.Add(world);
      created = true;
    }
    else
    {
      world.Update(payload.Key, payload.Name, payload.Description, _context.UserId);
    }

    await _worldRepository.EnsureUnicityAsync(world, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);

    WorldModel model = await _worldRepository.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, created);
  }
}
