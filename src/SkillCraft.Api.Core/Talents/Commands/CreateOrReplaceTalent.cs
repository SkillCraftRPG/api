using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record CreateOrReplaceTalentCommand(CreateOrReplaceTalentPayload Payload, Guid? Id) : ICommand<CreateOrReplaceTalentResult>;

internal class CreateOrReplaceTalentCommandHandler : ICommandHandler<CreateOrReplaceTalentCommand, CreateOrReplaceTalentResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentRepository _talentRepository;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceTalentCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ITalentRepository talentRepository,
    IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _talentRepository = talentRepository;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceTalentResult> HandleAsync(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentPayload payload = command.Payload;
    payload.Validate();

    Talent? talent = null;
    if (command.Id.HasValue)
    {
      talent = await _talentRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldId;

    Talent? requiredTalent = null;
    if (payload.RequiredTalentId.HasValue)
    {
      requiredTalent = await _talentRepository.LoadAsync(payload.RequiredTalentId.Value, cancellationToken)
        ?? throw new ResourceNotFoundException(new ResourceIdentifier(Talent.ResourceKind, payload.RequiredTalentId.Value, worldId), nameof(Talent.RequiredTalentId));
    }

    TalentSnapshot? snapshot = null;
    if (talent is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateTalent, world, cancellationToken);

      talent = new Talent(world, payload.Tier, command.Id, userId);
      _talentRepository.Add(talent);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

      if (payload.Tier != talent.Tier)
      {
        throw new ImmutablePropertyException<int>(talent, talent.Tier, payload.Tier, nameof(Talent.Tier));
      }

      snapshot = new TalentSnapshot(talent);
    }

    talent.Name = payload.Name.Trim();
    talent.Summary = payload.Summary?.CleanTrim();
    talent.HtmlContent = payload.HtmlContent?.CleanTrim();

    talent.SetAllowMultiplePurchases(payload.AllowMultiplePurchases);
    talent.SetSkill(payload.Skill);
    talent.SetRequiredTalent(requiredTalent);

    if (snapshot is not null)
    {
      TalentUpdated? record = snapshot.Compare(talent);
      if (record is not null)
      {
        talent.Update(userId);
        _talentRepository.Update(talent, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    TalentModel model = await _talentRepository.ReadAsync(talent, cancellationToken);
    return new CreateOrReplaceTalentResult(model, Created: snapshot is null);
  }
}
