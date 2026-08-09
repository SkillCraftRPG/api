using Logitar.CQRS;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;
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
    _talentRepository = talentRepository;
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

    Lineage? parent = null;
    if (lineage.ParentId.HasValue)
    {
      parent = await _lineageRepository.LoadAsync(lineage.ParentId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The lineage 'Id={lineage.ParentId}' was not found.");
    }
    else if (await _lineageQuerier.HasChildrenAsync(lineage, cancellationToken))
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    IReadOnlyCollection<Language> languages = await LoadLanguagesAsync(worldId, payload.LanguageIds, nameof(payload.LanguageIds), cancellationToken);

    Name name = new(payload.Name);
    IReadOnlyCollection<Customization> customizations = await LoadCustomizationsAsync(worldId, payload.CustomizationIds, nameof(payload.CustomizationIds), cancellationToken);

    CasteId casteId = new(worldId, payload.CasteId);
    Caste caste = await _casteRepository.LoadAsync(casteId, cancellationToken) ?? throw new CasteNotFoundException(casteId, nameof(payload.CasteId));

    EducationId educationId = new(worldId, payload.EducationId);
    Education education = await _educationRepository.LoadAsync(educationId, cancellationToken) ?? throw new EducationNotFoundException(educationId, nameof(payload.EducationId));

    IReadOnlyCollection<CharacterTalent> talents = await LoadTalentsAsync(worldId, payload.Talents, nameof(payload.Talents), cancellationToken);

    IReadOnlyDictionary<Skill, int> skills = payload.Skills.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.Sum(y => y.Rank)).AsReadOnly();

    Character character = new(
      CharacterId.NewId(worldId),
      lineage,
      name,
      caste,
      education,
      parent,
      languages,
      payload.DominantHand,
      customizations,
      talents,
      new StartingAttributes(payload.Attributes),
      skills,
      new CharacterAppearance(payload.Appearance),
      payload.Alignment,
      new CharacterPersonality(payload.Personality),
      Background.TryCreate(payload.Background),
      _context.ActorId);

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
      throw new CustomizationsNotFoundException(missingIds, propertyName);
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
      throw new LanguagesNotFoundException(missingIds, propertyName);
    }

    return languages;
  }

  private async Task<IReadOnlyCollection<CharacterTalent>> LoadTalentsAsync(
    WorldId worldId,
    IEnumerable<AddCharacterTalentPayload> payloads,
    string propertyName,
    CancellationToken cancellationToken)
  {
    HashSet<TalentId> talentIds = payloads.Select(payload => new TalentId(worldId, payload.TalentId)).ToHashSet();
    IReadOnlyCollection<Talent> talents = await _talentRepository.LoadAsync(talentIds, cancellationToken);

    IEnumerable<TalentId> missingIds = talentIds.Except(talents.Select(talent => talent.Id));
    if (missingIds.Any())
    {
      throw new TalentsNotFoundException(missingIds, propertyName);
    }

    Dictionary<Guid, Talent> talentsById = talents.ToDictionary(x => x.ResourceId, x => x);
    List<CharacterTalent> characterTalents = new(capacity: payloads.Count());
    foreach (AddCharacterTalentPayload payload in payloads)
    {
      characterTalents.Add(new CharacterTalent(talentsById[payload.TalentId], Name.TryCreate(payload.Qualifier), Notes.TryCreate(payload.Notes)));
    }
    return characterTalents.AsReadOnly();
  }
}
