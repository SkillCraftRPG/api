using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record CreateCharacterCommand(CreateCharacterPayload Payload) : ICommand<CharacterModel>;

internal class CreateCharacterCommandHandler : ICommandHandler<CreateCharacterCommand, CharacterModel>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public CreateCharacterCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IContext context,
    ICustomizationRepository customizationRepository,
    ILanguageRepository languageRepository,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _context = context;
    _customizationRepository = customizationRepository;
    _languageRepository = languageRepository;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<CharacterModel> HandleAsync(CreateCharacterCommand command, CancellationToken cancellationToken)
  {
    CreateCharacterPayload payload = command.Payload;
    payload.Validate();

    await _permissionService.CheckAsync(Actions.CreateCharacter, cancellationToken);

    WorldId worldId = _context.WorldId;

    LineageId lineageId = new(worldId, payload.LineageId);
    Lineage lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken)
      ?? throw new LineageNotFoundException(lineageId, nameof(payload.LineageId));
    int extraLanguages = lineage.Languages.Extra;
    HashSet<LanguageId> grantedLanguageIds = lineage.Languages.Ids.ToHashSet();

    Lineage? parent = null;
    if (lineage.ParentId.HasValue)
    {
      parent = await _lineageRepository.LoadAsync(lineage.ParentId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The lineage 'Id={lineage.ParentId}' was not found.");
      extraLanguages += parent.Languages.Extra;
      grantedLanguageIds.AddRange(parent.Languages.Ids);
    }
    else if (await _lineageQuerier.HasChildrenAsync(lineage, cancellationToken))
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    IReadOnlyCollection<Language> languages = await LoadLanguagesAsync(worldId, payload.LanguageIds, nameof(payload.LanguageIds), cancellationToken);
    IEnumerable<Language> grantedLanguages = languages.Where(language => grantedLanguageIds.Contains(language.Id));
    if (grantedLanguages.Any())
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    else if (languages.Count != extraLanguages)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    Name name = new(payload.Name);
    IReadOnlyCollection<Customization> customizations = await LoadCustomizationsAsync(worldId, payload.CustomizationIds, nameof(payload.CustomizationIds), cancellationToken);

    Character character = new(CharacterId.NewId(worldId), name, lineage, customizations, languages, _context.ActorId);

    await _characterRepository.SaveAsync(character, cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }

  private async Task<IReadOnlyCollection<Customization>> LoadCustomizationsAsync(WorldId worldId, IEnumerable<Guid> ids, string propertyName, CancellationToken cancellationToken)
  {
    HashSet<CustomizationId> customizationIds = ids.Select(id => new CustomizationId(worldId, id)).ToHashSet();
    IReadOnlyCollection<Customization> customizations = await _customizationRepository.LoadAsync(customizationIds, cancellationToken);

    IEnumerable<CustomizationId> missingIds = customizationIds.Except(customizations.Select(customization => customization.Id));
    if (missingIds.Any())
    {
      throw new CustomizationsNotFoundException(customizationIds, propertyName);
    }

    return customizations;
  }

  private async Task<IReadOnlyCollection<Language>> LoadLanguagesAsync(WorldId worldId, IEnumerable<Guid> ids, string propertyName, CancellationToken cancellationToken)
  {
    HashSet<LanguageId> languageIds = ids.Select(id => new LanguageId(worldId, id)).ToHashSet();
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(languageIds, cancellationToken);

    IEnumerable<LanguageId> missingIds = languageIds.Except(languages.Select(language => language.Id));
    if (missingIds.Any())
    {
      throw new LanguagesNotFoundException(languageIds, propertyName);
    }

    return languages;
  }
}
