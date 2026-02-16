using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Lineages.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : ICommand<LineageModel?>;

internal class UpdateLineageCommandHandler : ICommandHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateLineageCommandHandler(
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

  public async Task<LineageModel?> HandleAsync(UpdateLineageCommand command, CancellationToken cancellationToken)
  {
    UpdateLineagePayload payload = command.Payload;
    new UpdateLineageValidator().ValidateAndThrow(payload);

    LineageId lineageId = new(command.Id, _context.WorldId);
    Lineage? lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken);
    if (lineage is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      lineage.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      lineage.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      lineage.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Speeds is not null)
    {
      lineage.Speeds = new Speeds(payload.Speeds);
    }
    if (payload.Size is not null)
    {
      lineage.Size = new Size(payload.Size.Category, Roll.TryCreate(payload.Size.Height));
    }
    if (payload.Weight is not null)
    {
      lineage.Weight = new Weight(
        Roll.TryCreate(payload.Weight.Malnutrition),
        Roll.TryCreate(payload.Weight.Skinny),
        Roll.TryCreate(payload.Weight.Normal),
        Roll.TryCreate(payload.Weight.Overweight),
        Roll.TryCreate(payload.Weight.Obese));
    }
    if (payload.Age is not null)
    {
      lineage.Age = new Age(payload.Age);
    }

    lineage.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      lineage,
      async () => await _lineageRepository.SaveAsync(lineage, cancellationToken),
      cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }
}
