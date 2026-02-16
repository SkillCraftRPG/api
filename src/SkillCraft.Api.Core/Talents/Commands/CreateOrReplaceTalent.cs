using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Validators;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record CreateOrReplaceTalentCommand(CreateOrReplaceTalentPayload Payload, Guid? Id) : ICommand<CreateOrReplaceTalentResult>;

internal class CreateOrReplaceTalentCommandHandler : ICommandHandler<CreateOrReplaceTalentCommand, CreateOrReplaceTalentResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;
  private readonly IStorageService _storageService;

  public CreateOrReplaceTalentCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
    _storageService = storageService;
  }

  public async Task<CreateOrReplaceTalentResult> HandleAsync(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentPayload payload = command.Payload;
    new CreateOrReplaceTalentValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    TalentId talentId = TalentId.NewId(worldId);
    Talent? talent = null;
    if (command.Id.HasValue)
    {
      talentId = new(command.Id.Value, worldId);
      talent = await _talentRepository.LoadAsync(talentId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (talent is null)
    {
      await _permissionService.CheckAsync(Actions.CreateTalent, cancellationToken);

      talent = new Talent(worldId, name, userId, talentId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, talent, cancellationToken);

      talent.Name = name;
    }

    talent.Summary = Summary.TryCreate(payload.Summary);
    talent.Description = Description.TryCreate(payload.Description);

    talent.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      talent,
      async () => await _talentRepository.SaveAsync(talent, cancellationToken),
      cancellationToken);

    TalentModel model = await _talentQuerier.ReadAsync(talent, cancellationToken);
    return new CreateOrReplaceTalentResult(model, created);
  }
}
