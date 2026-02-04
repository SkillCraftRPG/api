using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Customizations.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record UpdateCustomizationCommand(Guid Id, UpdateCustomizationPayload Payload) : ICommand<CustomizationModel?>;

internal class UpdateCustomizationCommandHandler : ICommandHandler<UpdateCustomizationCommand, CustomizationModel?>
{
  private readonly IContext _context;
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateCustomizationCommandHandler(
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

  public async Task<CustomizationModel?> HandleAsync(UpdateCustomizationCommand command, CancellationToken cancellationToken)
  {
    UpdateCustomizationPayload payload = command.Payload;
    new UpdateCustomizationValidator().ValidateAndThrow(payload);

    CustomizationId customizationId = new(command.Id, _context.WorldId);
    Customization? customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken);
    if (customization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      customization.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      customization.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      customization.Description = Description.TryCreate(payload.Description.Value);
    }

    customization.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      customization,
      async () => await _customizationRepository.SaveAsync(customization, cancellationToken),
      cancellationToken);

    return await _customizationQuerier.ReadAsync(customization, cancellationToken);
  }
}
