using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record CreateOrReplaceLineageCommand(CreateOrReplaceLineagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLineageResult>;

internal class CreateOrReplaceLineageCommandHandler : ICommandHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceLineageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _context = context;
    _languageRepository = languageRepository;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceLineageResult> HandleAsync(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineagePayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      LineageId parentId = new(worldId, payload.ParentId.Value);
      parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
        ?? throw new LineageNotFoundException(parentId, nameof(payload.ParentId));
    }

    Lineage? lineage = null;
    LineageId lineageId = LineageId.NewId(worldId);
    if (command.Id.HasValue)
    {
      lineageId = new LineageId(worldId, command.Id.Value);
      lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (lineage is null)
    {
      World world = await _worldRepository.LoadFromContextAsync(cancellationToken);
      await _permissionService.CheckAsync(Actions.CreateLineage, world, cancellationToken);

      lineage = new Lineage(lineageId, name, parent, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

      lineage.Rename(name, actorId);
    }

    lineage.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);

    IEnumerable<Feature> features = payload.Features.Select(feature => FeatureHelper.Create(feature.Name, feature.Content));
    lineage.SetFeatures(features, actorId);

    LineageLanguages languages = await LineageHelper.GetLanguagesAsync(_languageRepository, worldId, payload.Languages, cancellationToken);
    lineage.SetLanguages(languages, actorId);

    LineageNames names = LineageHelper.GetNames(payload.Names);
    lineage.SetNames(names, actorId);

    LineageSpeeds speeds = new(payload.Speeds);
    lineage.SetSpeeds(speeds, actorId);

    LineageSize size = LineageHelper.GetSize(payload.Size);
    LineageWeight weight = LineageHelper.GetWeight(payload.Weight);
    LineageAge age = new(payload.Age);
    lineage.SetTraits(size, weight, age, actorId);

    await _lineageRepository.SaveAsync(lineage, cancellationToken);

    LineageModel model = await _lineageQuerier.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageResult(model, created);
  }
}
