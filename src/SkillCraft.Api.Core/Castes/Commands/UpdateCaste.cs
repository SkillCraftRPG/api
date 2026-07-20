using Logitar.CQRS;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record UpdateCasteCommand(Guid Id, UpdateCastePayload Payload) : ICommand<CasteModel?>;

internal class UpdateCasteCommandHandler : ICommandHandler<UpdateCasteCommand, CasteModel?>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public UpdateCasteCommandHandler(ICasteRepository casteRepository, IContext context, IPermissionService permissionService)
  {
    _casteRepository = casteRepository;
    _context = context;
    _permissionService = permissionService;
  }

  public async Task<CasteModel?> HandleAsync(UpdateCasteCommand command, CancellationToken cancellationToken)
  {
    UpdateCastePayload payload = command.Payload;
    payload.Validate();

    Caste? caste = await _casteRepository.LoadAsync(command.Id, cancellationToken);
    if (caste is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

    Feature? feature = null;
    if (payload.Feature is null)
    {
      if (caste.FeatureName is not null)
      {
        feature = new Feature(caste.FeatureName, caste.FeatureHtmlContent);
      }
    }
    else if (payload.Feature.Value is not null)
    {
      feature = new Feature(payload.Feature.Value);
    }

    CasteUpdated record = caste.Update(
      string.IsNullOrWhiteSpace(payload.Name) ? caste.Name : payload.Name,
      payload.Summary is null ? caste.Summary : payload.Summary.Value,
      payload.HtmlContent is null ? caste.HtmlContent : payload.HtmlContent.Value,
      payload.Skill is null ? caste.Skill : payload.Skill.Value,
      payload.WealthRoll is null ? caste.WealthRoll : payload.WealthRoll.Value,
      feature,
      _context.UserId);
    _casteRepository.Update(caste, record);

    await _context.SaveChangesAsync(cancellationToken);

    return await _casteRepository.ReadAsync(caste, cancellationToken);
  }
}
