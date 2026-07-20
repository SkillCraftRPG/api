using Logitar.CQRS;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record CreateOrReplaceCasteCommand(CreateOrReplaceCastePayload Payload, Guid? Id) : ICommand<CreateOrReplaceCasteResult>;

internal class CreateOrReplaceCasteCommandHandler : ICommandHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceCasteCommandHandler(
    ICasteRepository casteRepository,
    IContext context,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _casteRepository = casteRepository;
    _context = context;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceCasteResult> HandleAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCastePayload payload = command.Payload;
    payload.Validate();

    Caste? caste = null;
    if (command.Id.HasValue)
    {
      caste = await _casteRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    bool created = false;
    if (caste is null)
    {
      World world = await _worldRepository.LoadAsync(_context.WorldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={_context.WorldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateCaste, world, cancellationToken);

      caste = new Caste(world, payload.Name, command.Id, payload.Summary, payload.HtmlContent, payload.Skill, payload.WealthRoll, _context.UserId);
      _casteRepository.Add(caste);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

      CasteUpdated record = caste.Update(payload.Name, payload.Summary, payload.HtmlContent, payload.Skill, payload.WealthRoll, _context.UserId);
      _casteRepository.Update(caste, record);
    }

    await _context.SaveChangesAsync(cancellationToken);

    CasteModel model = await _casteRepository.ReadAsync(caste, cancellationToken);
    return new CreateOrReplaceCasteResult(model, created);
  }
}
