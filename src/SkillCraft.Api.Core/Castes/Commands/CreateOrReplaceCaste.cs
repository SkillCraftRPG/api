using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Castes.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record CreateOrReplaceCasteCommand(CreateOrReplaceCastePayload Payload, Guid? Id) : ICommand<CreateOrReplaceCasteResult>;

internal class CreateOrReplaceCasteCommandHandler : ICommandHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>
{
  private readonly IContext _context;
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplaceCasteCommandHandler(
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

  public async Task<CreateOrReplaceCasteResult> HandleAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCastePayload payload = command.Payload;
    new CreateOrReplaceCasteValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    CasteId casteId = CasteId.NewId(worldId);
    Caste? caste = null;
    if (command.Id.HasValue)
    {
      casteId = new(command.Id.Value, worldId);
      caste = await _casteRepository.LoadAsync(casteId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (caste is null)
    {
      await _permissionService.CheckAsync(Actions.CreateCaste, cancellationToken);

      caste = new Caste(worldId, name, userId, casteId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

      caste.Name = name;
    }

    caste.Summary = Summary.TryCreate(payload.Summary);
    caste.Description = Description.TryCreate(payload.Description);

    caste.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      caste,
      async () => await _casteRepository.SaveAsync(caste, cancellationToken),
      cancellationToken);

    CasteModel model = await _casteQuerier.ReadAsync(caste, cancellationToken);
    return new CreateOrReplaceCasteResult(model, created);
  }
}
