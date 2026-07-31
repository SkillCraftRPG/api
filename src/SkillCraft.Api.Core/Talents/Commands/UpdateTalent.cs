using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record UpdateTalentCommand(Guid Id, UpdateTalentPayload Payload) : ICommand<TalentModel?>;

internal class UpdateTalentCommandHandler : ICommandHandler<UpdateTalentCommand, TalentModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentRepository _talentRepository;

  public UpdateTalentCommandHandler(IContext context, IPermissionService permissionService, ITalentRepository talentRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _talentRepository = talentRepository;
  }

  public async Task<TalentModel?> HandleAsync(UpdateTalentCommand command, CancellationToken cancellationToken)
  {
    UpdateTalentPayload payload = command.Payload;
    payload.Validate();

    Talent? talent = await _talentRepository.LoadAsync(command.Id, cancellationToken);
    if (talent is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

    TalentSnapshot snapshot = new(talent);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      talent.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      talent.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.HtmlContent is not null)
    {
      talent.HtmlContent = payload.HtmlContent.Value?.CleanTrim();
    }

    if (payload.AllowMultiplePurchases.HasValue)
    {
      talent.SetAllowMultiplePurchases(payload.AllowMultiplePurchases.Value);
    }
    if (payload.Skill is not null)
    {
      talent.SetSkill(payload.Skill.Value);
    }
    if (payload.RequiredTalentId is not null)
    {
      Talent? requiredTalent = null;
      if (payload.RequiredTalentId.Value.HasValue)
      {
        requiredTalent = await _talentRepository.LoadAsync(payload.RequiredTalentId.Value.Value, cancellationToken)
          ?? throw new ResourceNotFoundException(new ResourceIdentifier(Talent.ResourceKind, payload.RequiredTalentId.Value.Value, _context.WorldId), nameof(Talent.RequiredTalentId));
      }
      talent.SetRequiredTalent(requiredTalent);
    }

    TalentUpdated? record = snapshot.Compare(talent);
    if (record is not null)
    {
      talent.Update(_context.UserId);
      _talentRepository.Update(talent, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _talentRepository.ReadAsync(talent, cancellationToken);
  }
}
