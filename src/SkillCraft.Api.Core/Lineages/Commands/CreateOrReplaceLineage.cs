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

    Guid userId = _context.UserUid;
    Guid worldId = _context.WorldUid;

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      parent = await _lineageRepository.LoadAsync(payload.ParentId.Value, cancellationToken)
        ?? throw new ResourceNotFoundException(new ResourceIdentifier(Lineage.ResourceKind, payload.ParentId.Value, worldId), nameof(payload.ParentId));
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
      World world = await _worldRepository.LoadFromContextAsync(cancellationToken);
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
    lineage.Content = payload.Content?.CleanTrim();

    lineage.SetLanguages(languages, payload.Languages.Extra, payload.Languages.Content);
    lineage.SetNames(payload.Names.Family, payload.Names.Female, payload.Names.Male, payload.Names.Unisex, payload.Names.Custom, payload.Names.Content);
    lineage.SetSpeeds(payload.Speeds);
    lineage.SetSize(payload.Size);
    lineage.SetWeight(payload.Weight);
    lineage.SetAge(payload.Age);

    if (snapshot is not null)
    {
      LineageUpdated? record = snapshot.Compare(lineage);
      if (record is not null)
      {
        lineage.Update(userId);
        _lineageRepository.Update(lineage, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    LineageModel model = await _lineageRepository.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageResult(model, Created: snapshot is null);
  }
}
