using FluentValidation;
using FluentValidation.Results;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Customizations.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record CreateOrReplaceCustomizationCommand(CreateOrReplaceCustomizationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceCustomizationResult>;

internal class CreateOrReplaceCustomizationCommandHandler : ICommandHandler<CreateOrReplaceCustomizationCommand, CreateOrReplaceCustomizationResult>
{
  private readonly IContext _context;
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplaceCustomizationCommandHandler(
    IContext context,
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<CreateOrReplaceCustomizationResult> HandleAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationPayload payload = command.Payload;
    new CreateOrReplaceCustomizationValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    CustomizationId customizationId = CustomizationId.NewId(worldId);
    Customization? customization = null;
    if (command.Id.HasValue)
    {
      customizationId = new(command.Id.Value, worldId);
      customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (customization is null)
    {
      await _permissionService.CheckAsync(Actions.CreateCustomization, cancellationToken);

      customization = new Customization(worldId, payload.Kind, name, userId, customizationId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

      if (payload.Kind != customization.Kind)
      {
        ValidationFailure failure = new(nameof(payload.Kind), "The customization kind cannot be changed.", payload.Kind)
        {
          CustomState = new { customization.Kind },
          ErrorCode = "CustomizationKindCannotBeChanged"
        };
        throw new ValidationException([failure]);
      }

      customization.Name = name;
    }

    customization.Summary = Summary.TryCreate(payload.Summary);
    customization.Description = Description.TryCreate(payload.Description);

    customization.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      customization,
      async () => await _customizationRepository.SaveAsync(customization, cancellationToken),
      cancellationToken);

    CustomizationModel model = await _customizationQuerier.ReadAsync(customization, cancellationToken);
    return new CreateOrReplaceCustomizationResult(model, created);
  }
}
