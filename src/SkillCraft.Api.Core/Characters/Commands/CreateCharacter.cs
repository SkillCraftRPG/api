using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters.Validators;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record CreateCharacterCommand(CreateCharacterPayload Payload) : ICommand<CharacterModel>;

internal class CreateCharacterCommandHandler : ICommandHandler<CreateCharacterCommand, CharacterModel>
{
  private readonly ICasteRepository _casteRepository;
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IEducationRepository _educationRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;
  private readonly ITalentRepository _talentRepository;

  public CreateCharacterCommandHandler(
    ICasteRepository casteRepository,
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IContext context,
    ICustomizationRepository customizationRepository,
    IEducationRepository educationRepository,
    ILanguageRepository languageRepository,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IStorageService storageService,
    ITalentRepository talentRepository)
  {
    _casteRepository = casteRepository;
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _context = context;
    _customizationRepository = customizationRepository;
    _educationRepository = educationRepository;
    _languageRepository = languageRepository;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _storageService = storageService;
    _talentRepository = talentRepository;
  }

  public async Task<CharacterModel> HandleAsync(CreateCharacterCommand command, CancellationToken cancellationToken)
  {
    CreateCharacterPayload payload = command.Payload;
    new CreateCharacterValidator().ValidateAndThrow(payload);

    await _permissionService.CheckAsync(Actions.CreateCharacter, cancellationToken);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    Name name = new(payload.Name);
    Characteristics characteristics = new(payload.Characteristics);
    StartingAttributes startingAttributes = new(payload.StartingAttributes);

    Lineage lineage = await FindLineageAsync(payload, worldId, cancellationToken);
    Caste caste = await FindCasteAsync(payload, worldId, cancellationToken);
    Education education = await FindEducationAsync(payload, worldId, cancellationToken);

    IReadOnlyCollection<Language> languages = await FindLanguagesAsync(payload, lineage, cancellationToken);
    IReadOnlyCollection<Customization> customizations = await FindCustomizationsAsync(payload, worldId, cancellationToken);
    IReadOnlyCollection<Talent> talents = await FindTalentsAsync(payload, worldId, cancellationToken);

    Character character = new(worldId, name, lineage, caste, education, talents, userId, characteristics, startingAttributes, languages, customizations);

    await _storageService.ExecuteWithQuotaAsync(
      character,
      async () => await _characterRepository.SaveAsync(character, cancellationToken),
      cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }

  private async Task<Caste> FindCasteAsync(CreateCharacterPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    CasteId casteId = new(payload.CasteId, worldId);
    return await _casteRepository.LoadAsync(casteId, cancellationToken)
      ?? throw new EntityNotFoundException(new Entity(Caste.EntityKind, payload.CasteId, worldId), nameof(payload.CasteId));
  }

  private async Task<IReadOnlyCollection<Customization>> FindCustomizationsAsync(CreateCharacterPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    HashSet<CustomizationId> customizationIds = payload.CustomizationIds.Select(customizationId => new CustomizationId(customizationId, worldId)).ToHashSet();
    IReadOnlyCollection<Customization> customizations = await _customizationRepository.LoadAsync(customizationIds, cancellationToken);
    HashSet<Guid> missingIds = customizationIds.Except(customizations.Select(customization => customization.Id)).Select(id => id.EntityId).ToHashSet();
    if (missingIds.Count > 0)
    {
      throw new CustomizationsNotFoundException(worldId, missingIds, nameof(payload.CustomizationIds));
    }
    return customizations;
  }

  private async Task<Education> FindEducationAsync(CreateCharacterPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    EducationId educationId = new(payload.EducationId, worldId);
    return await _educationRepository.LoadAsync(educationId, cancellationToken)
      ?? throw new EntityNotFoundException(new Entity(Education.EntityKind, payload.EducationId, worldId), nameof(payload.EducationId));
  }

  private async Task<IReadOnlyCollection<Language>> FindLanguagesAsync(CreateCharacterPayload payload, Lineage lineage, CancellationToken cancellationToken)
  {
    WorldId worldId = lineage.WorldId;
    HashSet<LanguageId> languageIds = payload.LanguageIds.Select(languageId => new LanguageId(languageId, worldId)).ToHashSet();
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(languageIds, cancellationToken);
    HashSet<Guid> missingIds = languageIds.Except(languages.Select(language => language.Id)).Select(id => id.EntityId).ToHashSet();
    if (missingIds.Count > 0)
    {
      throw new LanguagesNotFoundException(worldId, missingIds, nameof(payload.LanguageIds));
    }
    return languages;
  }

  private async Task<Lineage> FindLineageAsync(CreateCharacterPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    LineageId lineageId = new(payload.LineageId, worldId);
    Lineage lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken)
      ?? throw new EntityNotFoundException(new Entity(Lineage.EntityKind, payload.LineageId, worldId), nameof(payload.LineageId));
    if (await _lineageQuerier.HasChildrenAsync(lineage, cancellationToken))
    {
      throw new InvalidLineageException(lineage, nameof(payload.LineageId));
    }
    return lineage;
  }

  private async Task<IReadOnlyCollection<Talent>> FindTalentsAsync(CreateCharacterPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    HashSet<TalentId> talentIds = payload.TalentIds.Select(talentId => new TalentId(talentId, worldId)).ToHashSet();
    IReadOnlyCollection<Talent> talents = await _talentRepository.LoadAsync(talentIds, cancellationToken);
    HashSet<Guid> missingIds = talentIds.Except(talents.Select(talent => talent.Id)).Select(id => id.EntityId).ToHashSet();
    if (missingIds.Count > 0)
    {
      throw new TalentsNotFoundException(worldId, missingIds, nameof(payload.TalentIds));
    }
    return talents;
  }
}
