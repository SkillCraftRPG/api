using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Castes.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record UpdateCasteCommand(Guid Id, UpdateCastePayload Payload) : ICommand<CasteModel?>;

internal class UpdateCasteCommandHandler : ICommandHandler<UpdateCasteCommand, CasteModel?>
{
  private readonly IContext _context;
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateCasteCommandHandler(
    IContext context,
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<CasteModel?> HandleAsync(UpdateCasteCommand command, CancellationToken cancellationToken)
  {
    UpdateCastePayload payload = command.Payload;
    new UpdateCasteValidator().ValidateAndThrow(payload);

    CasteId casteId = new(command.Id, _context.WorldId);
    Caste? caste = await _casteRepository.LoadAsync(casteId, cancellationToken);
    if (caste is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      caste.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      caste.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      caste.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Skill is not null)
    {
      caste.Skill = payload.Skill.Value;
    }
    if (payload.WealthRoll is not null)
    {
      caste.WealthRoll = Roll.TryCreate(payload.WealthRoll.Value);
    }
    if (payload.Feature is not null)
    {
      caste.Feature = payload.Feature.Value is null ? null : new Feature(new Name(payload.Feature.Value.Name), Description.TryCreate(payload.Feature.Value.Description));
    }

    caste.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      caste,
      async () => await _casteRepository.SaveAsync(caste, cancellationToken),
      cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }
}
