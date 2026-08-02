using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record CreateOrReplaceCustomizationCommand(CreateOrReplaceCustomizationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceCustomizationResult>;

internal class CreateOrReplaceCustomizationCommandHandler : ICommandHandler<CreateOrReplaceCustomizationCommand, CreateOrReplaceCustomizationResult>
{
  private readonly IContext _context;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceCustomizationCommandHandler(
    IContext context,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _context = context;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceCustomizationResult> HandleAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationPayload payload = command.Payload;
    payload.Validate();

    Customization? customization = null;
    if (command.Id.HasValue)
    {
      customization = await _customizationRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserUid;

    CustomizationSnapshot? snapshot = null;
    if (customization is null)
    {
      World world = await _worldRepository.LoadFromContextAsync(cancellationToken);
      await _permissionService.CheckAsync(Actions.CreateCustomization, world, cancellationToken);

      customization = new Customization(world, payload.Kind, command.Id, userId);
      _customizationRepository.Add(customization);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

      if (payload.Kind != customization.Kind)
      {
        throw new ImmutablePropertyException<CustomizationKind>(customization, customization.Kind, payload.Kind, nameof(Customization.Kind));
      }

      snapshot = new CustomizationSnapshot(customization);
    }

    customization.Name = payload.Name.Trim();
    customization.Summary = payload.Summary?.CleanTrim();
    customization.Content = payload.Content?.CleanTrim();

    if (snapshot is not null)
    {
      CustomizationUpdated? record = snapshot.Compare(customization);
      if (record is not null)
      {
        customization.Update(userId);
        _customizationRepository.Update(customization, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    CustomizationModel model = await _customizationRepository.ReadAsync(customization, cancellationToken);
    return new CreateOrReplaceCustomizationResult(model, Created: snapshot is null);
  }
}
