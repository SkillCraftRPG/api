using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents.Validators;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record UpdateTalentCommand(Guid Id, UpdateTalentPayload Payload) : ICommand<TalentModel?>;

internal class UpdateTalentCommandHandler : SaveTalent, ICommandHandler<UpdateTalentCommand, TalentModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly IStorageService _storageService;

  public UpdateTalentCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository,
    IStorageService storageService) : base(talentRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
    _storageService = storageService;
  }

  public async Task<TalentModel?> HandleAsync(UpdateTalentCommand command, CancellationToken cancellationToken)
  {
    UpdateTalentPayload payload = command.Payload;
    new UpdateTalentValidator().ValidateAndThrow(payload);

    WorldId worldId = _context.WorldId;

    TalentId talentId = new(command.Id, worldId);
    Talent? talent = await TalentRepository.LoadAsync(talentId, cancellationToken);
    if (talent is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      talent.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      talent.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      talent.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.AllowMultiplePurchases.HasValue)
    {
      talent.AllowMultiplePurchases = payload.AllowMultiplePurchases.Value;
    }
    if (payload.Skill is not null)
    {
      talent.Skill = payload.Skill.Value;
    }
    if (payload.RequiredTalentId is not null)
    {
      await SetRequiredTalentAsync(talent, payload.RequiredTalentId.Value, worldId, cancellationToken);
    }

    talent.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      talent,
      async () => await TalentRepository.SaveAsync(talent, cancellationToken),
      cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
