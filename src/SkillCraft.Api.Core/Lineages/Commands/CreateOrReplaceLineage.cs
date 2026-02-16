using FluentValidation;
using FluentValidation.Results;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Lineages.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record CreateOrReplaceLineageCommand(CreateOrReplaceLineagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLineageResult>;

internal class CreateOrReplaceLineageCommandHandler : ICommandHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>
{
  private readonly IContext _context;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplaceLineageCommandHandler(
    IContext context,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<CreateOrReplaceLineageResult> HandleAsync(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineagePayload payload = command.Payload;
    new CreateOrReplaceLineageValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    LineageId lineageId = LineageId.NewId(worldId);
    Lineage? lineage = null;
    if (command.Id.HasValue)
    {
      lineageId = new(command.Id.Value, worldId);
      lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (lineage is null)
    {
      await _permissionService.CheckAsync(Actions.CreateLineage, cancellationToken);

      Lineage? parent = null;
      if (payload.ParentId.HasValue)
      {
        LineageId parentId = new(payload.ParentId.Value, worldId);
        parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
          ?? throw new EntityNotFoundException(new Entity(Lineage.EntityKind, payload.ParentId.Value, worldId), nameof(payload.ParentId));
      }

      lineage = new Lineage(worldId, name, parent, userId, lineageId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

      if (payload.ParentId != lineage.ParentId?.EntityId)
      {
        ValidationFailure failure = new(nameof(payload.ParentId), "The lineage parent cannot be changed.", payload.ParentId)
        {
          CustomState = new { ParentId = lineage.ParentId?.EntityId },
          ErrorCode = "LineageParentCannotBeChanged"
        };
        throw new ValidationException([failure]);
      }

      lineage.Name = name;
    }

    lineage.Summary = Summary.TryCreate(payload.Summary);
    lineage.Description = Description.TryCreate(payload.Description);

    lineage.Speeds = new Speeds(payload.Speeds);
    lineage.Size = new Size(payload.Size.Category, payload.Size.Height);

    lineage.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      lineage,
      async () => await _lineageRepository.SaveAsync(lineage, cancellationToken),
      cancellationToken);

    LineageModel model = await _lineageQuerier.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageResult(model, created);
  }
}
