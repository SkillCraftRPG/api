using Logitar.CQRS;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record DeleteCustomizationCommand(Guid Id) : ICommand<CustomizationModel?>;

internal class DeleteCustomizationCommandHandler : ICommandHandler<DeleteCustomizationCommand, CustomizationModel?>
{
  private readonly IContext _context;
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteCustomizationCommandHandler(
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

  public async Task<CustomizationModel?> HandleAsync(DeleteCustomizationCommand command, CancellationToken cancellationToken)
  {
    CustomizationId customizationId = new(command.Id, _context.WorldId);
    Customization? customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken);
    if (customization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, customization, cancellationToken);
    CustomizationModel model = await _customizationQuerier.ReadAsync(customization, cancellationToken);

    customization.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      customization,
      async () => await _customizationRepository.SaveAsync(customization, cancellationToken),
      cancellationToken);

    return model;
  }
}
