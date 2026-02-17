using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : ICommand<LineageModel?>;

internal class UpdateLineageCommandHandler : SaveLineage, ICommandHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateLineageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IStorageService storageService) : base(languageRepository)
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

    WorldId worldId = _context.WorldId;

    LineageId lineageId = new(command.Id, worldId);
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

    if (payload.Features is not null)
    {
      lineage.SetFeatures(payload.Features.Select(feature => Feature.Create(feature.Name, feature.Description)));
    }
    if (payload.Languages is not null)
    {
      await SetLanguagesAsync(lineage, payload.Languages, worldId, cancellationToken);
    }
    if (payload.Names is not null)
    {
      SetNames(lineage, payload.Names);
    }

    if (payload.Speeds is not null)
    {
      lineage.Speeds = new Speeds(payload.Speeds);
    }
    if (payload.Size is not null)
    {
      lineage.Size = Size.Create(payload.Size.Category, payload.Size.Height);
    }
    if (payload.Weight is not null)
    {
      lineage.Weight = Weight.Create(payload.Weight.Malnutrition, payload.Weight.Skinny, payload.Weight.Normal, payload.Weight.Overweight, payload.Weight.Obese);
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
