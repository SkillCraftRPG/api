using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record CreateOrReplaceCustomizationCommand(CreateOrReplaceCustomizationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceCustomizationResult>;

internal class CreateOrReplaceCustomizationCommandHandler : ICommandHandler<CreateOrReplaceCustomizationCommand, CreateOrReplaceCustomizationResult>
{
  private readonly IContext _context;
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceCustomizationCommandHandler(
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

  public async Task<CreateOrReplaceCustomizationResult> HandleAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Customization? customization = null;
    CustomizationId customizationId = CustomizationId.NewId(worldId);
    if (command.Id.HasValue)
    {
      customizationId = new CustomizationId(worldId, command.Id.Value);
      customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (customization is null)
    {
      await _permissionService.CheckAsync(Actions.CreateCustomization, cancellationToken);

      customization = new Customization(customizationId, payload.Kind, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

      if (customization.Kind != payload.Kind)
      {
        throw new ImmutablePropertyException<CustomizationKind>(customization, customization.Kind, payload.Kind, nameof(payload.Kind));
      }

      customization.Rename(name, actorId);
    }

    customization.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);

    await _customizationRepository.SaveAsync(customization, cancellationToken);

    CustomizationModel model = await _customizationQuerier.ReadAsync(customization, cancellationToken);
    return new CreateOrReplaceCustomizationResult(model, created);
  }
}
