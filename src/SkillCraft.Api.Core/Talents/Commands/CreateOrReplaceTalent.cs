using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record CreateOrReplaceTalentCommand(CreateOrReplaceTalentPayload Payload, Guid? Id) : ICommand<CreateOrReplaceTalentResult>;

internal class CreateOrReplaceTalentCommandHandler : ICommandHandler<CreateOrReplaceTalentCommand, CreateOrReplaceTalentResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceTalentCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository,
    IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceTalentResult> HandleAsync(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Talent? talent = null;
    TalentId talentId = TalentId.NewId(worldId);
    if (command.Id.HasValue)
    {
      talentId = new TalentId(worldId, command.Id.Value);
      talent = await _talentRepository.LoadAsync(talentId, cancellationToken);
    }

    Name name = new(payload.Name);

    Talent? requiredTalent = null;
    if (payload.RequiredTalentId.HasValue)
    {
      TalentId requiredTalentId = new(worldId, payload.RequiredTalentId.Value);
      requiredTalent = await _talentRepository.LoadAsync(requiredTalentId, cancellationToken)
        ?? throw new TalentNotFoundException(requiredTalentId, nameof(payload.RequiredTalentId));
    }

    bool created = false;
    if (talent is null)
    {
      World world = await _worldRepository.LoadFromContextAsync(cancellationToken);
      await _permissionService.CheckAsync(Actions.CreateTalent, world, cancellationToken);

      talent = new Talent(talentId, new TalentTier(payload.Tier), name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

      if (talent.Tier.Value != payload.Tier)
      {
        throw new ImmutablePropertyException<int>(talent, talent.Tier.Value, payload.Tier, nameof(payload.Tier));
      }

      talent.Rename(name, actorId);
    }

    talent.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);
    talent.SetRules(payload.AllowMultiplePurchases, payload.Skill, actorId);
    talent.SetRequirements(requiredTalent, actorId);

    await _talentRepository.SaveAsync(talent, cancellationToken);

    TalentModel model = await _talentQuerier.ReadAsync(talent, cancellationToken);
    return new CreateOrReplaceTalentResult(model, created);
  }
}
