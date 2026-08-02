using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record UpdateTalentCommand(Guid Id, UpdateTalentPayload Payload) : ICommand<TalentModel?>;

internal class UpdateTalentCommandHandler : ICommandHandler<UpdateTalentCommand, TalentModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;

  public UpdateTalentCommandHandler(IContext context, IPermissionService permissionService, ITalentQuerier talentQuerier, ITalentRepository talentRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
  }

  public async Task<TalentModel?> HandleAsync(UpdateTalentCommand command, CancellationToken cancellationToken)
  {
    UpdateTalentPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    TalentId talentId = new(worldId, command.Id);
    Talent? talent = await _talentRepository.LoadAsync(talentId, cancellationToken);
    if (talent is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      talent.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      talent.Edit(
        payload.Summary is null ? talent.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? talent.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.AllowMultiplePurchases.HasValue || payload.Skill is not null)
    {
      talent.SetRules(payload.AllowMultiplePurchases ?? talent.AllowMultiplePurchases, payload.Skill?.Value ?? talent.Skill, actorId);
    }

    if (payload.RequiredTalentId is not null)
    {
      Talent? requiredTalent = null;
      if (payload.RequiredTalentId.Value.HasValue)
      {
        TalentId requiredTalentId = new(worldId, payload.RequiredTalentId.Value.Value);
        requiredTalent = await _talentRepository.LoadAsync(requiredTalentId, cancellationToken)
          ?? throw new TalentNotFoundException(requiredTalentId, nameof(payload.RequiredTalentId));
      }
      talent.SetRequirements(requiredTalent, actorId);
    }

    await _talentRepository.SaveAsync(talent, cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
