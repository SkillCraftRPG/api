using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Customizations.Commands;

internal record UpdateCustomizationCommand(Guid Id, UpdateCustomizationPayload Payload) : ICommand<CustomizationModel?>;

internal class UpdateCustomizationCommandHandler : ICommandHandler<UpdateCustomizationCommand, CustomizationModel?>
{
  private readonly IContext _context;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public UpdateCustomizationCommandHandler(IContext context, ICustomizationRepository customizationRepository, IPermissionService permissionService)
  {
    _context = context;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<CustomizationModel?> HandleAsync(UpdateCustomizationCommand command, CancellationToken cancellationToken)
  {
    UpdateCustomizationPayload payload = command.Payload;
    payload.Validate();

    Customization? customization = await _customizationRepository.LoadAsync(command.Id, cancellationToken);
    if (customization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, customization, cancellationToken);

    CustomizationSnapshot snapshot = new(customization);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      customization.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      customization.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.HtmlContent is not null)
    {
      customization.HtmlContent = payload.HtmlContent.Value?.CleanTrim();
    }

    CustomizationUpdated? record = snapshot.Compare(customization);
    if (record is not null)
    {
      customization.Update(_context.UserId);
      _customizationRepository.Update(customization, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _customizationRepository.ReadAsync(customization, cancellationToken);
  }
}
