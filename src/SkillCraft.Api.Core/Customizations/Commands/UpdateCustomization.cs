using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record UpdateCustomizationCommand(Guid Id, UpdateCustomizationPayload Payload) : ICommand<CustomizationModel?>;

internal class UpdateCustomizationCommandHandler : ICommandHandler<UpdateCustomizationCommand, CustomizationModel?>
{
  private readonly IContext _context;
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public UpdateCustomizationCommandHandler(
    IContext context,
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<CustomizationModel?> HandleAsync(UpdateCustomizationCommand command, CancellationToken cancellationToken)
  {
    UpdateCustomizationPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    CustomizationId customizationId = new(worldId, command.Id);
    Customization? customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken);
    if (customization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      customization.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      customization.Edit(
        payload.Summary is null ? customization.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? customization.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    await _customizationRepository.SaveAsync(customization, cancellationToken);

    return await _customizationQuerier.ReadAsync(customization, cancellationToken);
  }
}
