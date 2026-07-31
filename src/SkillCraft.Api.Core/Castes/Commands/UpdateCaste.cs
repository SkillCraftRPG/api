using Logitar;
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

    CasteSnapshot snapshot = new(caste);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      caste.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      caste.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.HtmlContent is not null)
    {
      caste.HtmlContent = payload.HtmlContent.Value?.CleanTrim();
    }

    if (payload.Skill is not null)
    {
      caste.Skill = payload.Skill.Value;
    }
    if (payload.WealthRoll is not null)
    {
      caste.WealthRoll = payload.WealthRoll.Value?.CleanTrim()?.ToLowerInvariant();
    }
    if (payload.Feature is not null)
    {
      caste.SetFeature(payload.Feature.Value is null ? null : new Feature(payload.Feature.Value));
    }

    CasteUpdated? record = snapshot.Compare(caste);
    if (record is not null)
    {
      caste.Update(_context.UserId);
      _casteRepository.Update(caste, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _casteRepository.ReadAsync(caste, cancellationToken);
  }
}
