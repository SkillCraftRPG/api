using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents.Validators;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record UpdateTalentCommand(Guid Id, UpdateTalentPayload Payload) : ICommand<TalentModel?>;

internal class UpdateTalentCommandHandler : ICommandHandler<UpdateTalentCommand, TalentModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;
  private readonly IStorageService _storageService;

  public UpdateTalentCommandHandler(
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

  public async Task<TalentModel?> HandleAsync(UpdateTalentCommand command, CancellationToken cancellationToken)
  {
    UpdateTalentPayload payload = command.Payload;
    new UpdateTalentValidator().ValidateAndThrow(payload);

    TalentId talentId = new(command.Id, _context.WorldId);
    Talent? talent = await _talentRepository.LoadAsync(talentId, cancellationToken);
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

    talent.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      talent,
      async () => await _talentRepository.SaveAsync(talent, cancellationToken),
      cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
