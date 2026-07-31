using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record CreateOrReplaceLineageCommand(CreateOrReplaceLineagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLineageResult>;

internal class CreateOrReplaceLineageCommandHandler : ICommandHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceLineageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _context = context;
    _languageRepository = languageRepository;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceLineageResult> HandleAsync(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineagePayload payload = command.Payload;
    payload.Validate();

    Lineage? lineage = null;
    if (command.Id.HasValue)
    {
      lineage = await _lineageRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldId;

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      parent = await _lineageRepository.LoadAsync(payload.ParentId.Value, cancellationToken)
        ?? throw new ResourceNotFoundException(new ResourceIdentifier(Lineage.ResourceKind, payload.ParentId.Value, worldId), nameof(payload.ParentId));

      if (parent.Parent is not null)
      {
        throw new InvalidParentLineageException(parent, nameof(payload.ParentId));
      }
    }

    IReadOnlyCollection<Language> languages = [];
    if (payload.Languages.Ids.Count > 0)
    {
      languages = await _languageRepository.LoadAsync(payload.Languages.Ids, cancellationToken);

      HashSet<Guid> missingIds = payload.Languages.Ids.Except(languages.Select(language => language.Id)).ToHashSet();
      if (missingIds.Count > 0)
      {
        string propertyName = string.Join('.', nameof(payload.Languages), nameof(payload.Languages.Ids));
        throw new LanguagesNotFoundException(worldId, missingIds, propertyName);
      }
    }

    LineageSnapshot? snapshot = null;
    if (lineage is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateLineage, world, cancellationToken);

      lineage = new Lineage(world, command.Id, parent, userId);
      _lineageRepository.Add(lineage);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

      if (parent?.Id != lineage.Parent?.Id)
      {
        throw new ImmutablePropertyException<Guid?>(lineage, lineage.Parent?.Id, parent?.Id, nameof(Lineage.ParentId));
      }

      snapshot = new LineageSnapshot(lineage);
    }

    lineage.Name = payload.Name.Trim();
    lineage.Summary = payload.Summary?.CleanTrim();
    lineage.HtmlContent = payload.HtmlContent?.CleanTrim();

    LineageHelper.SetLanguages(lineage, languages, payload.Languages);
    LineageHelper.SetNames(lineage, payload.Names);
    LineageHelper.SetSpeeds(lineage, payload.Speeds);
    LineageHelper.SetSize(lineage, payload.Size);
    LineageHelper.SetWeight(lineage, payload.Weight);
    LineageHelper.SetAge(lineage, payload.Age);

    if (snapshot is not null && snapshot.HasChanges)
    {
      lineage.Update(userId);
      // TODO(fpion): produce record
      // TODO(fpion): _lineageRepository.Update(lineage, record);
    }

    await _context.SaveChangesAsync(cancellationToken);

    LineageModel model = await _lineageRepository.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageResult(model, Created: snapshot is null);
  }
}
