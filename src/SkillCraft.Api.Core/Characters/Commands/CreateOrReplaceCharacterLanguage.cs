using Logitar.CQRS;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record CreateOrReplaceCharacterLanguageCommand(Guid CharacterId, Guid LanguageId, CreateOrReplaceCharacterLanguagePayload Payload)
  : ICommand<CreateOrReplaceCharacterLanguageResult?>;

internal class CreateOrReplaceCharacterLanguageCommandHandler : ICommandHandler<CreateOrReplaceCharacterLanguageCommand, CreateOrReplaceCharacterLanguageResult?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceCharacterLanguageCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IContext context,
    ILanguageRepository languageRepository,
    ILineageRepository lineageRepository,
    IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _context = context;
    _languageRepository = languageRepository;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceCharacterLanguageResult?> HandleAsync(CreateOrReplaceCharacterLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterLanguagePayload payload = command.Payload;
    payload.Validate();

    CharacterId characterId = new(_context.WorldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, character, cancellationToken);

    CharacterLanguageAcquisition acquisition = new(payload.Source, payload.Target, Notes.TryCreate(payload.Notes));
    Ascendancy? ascendancy = null;

    LanguageId languageId = new(character.WorldId, command.LanguageId);
    CharacterLanguageAcquisition? existingAcquisition = character.TryGetLanguage(languageId);
    if (existingAcquisition is null)
    {
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, nameof(command.LanguageId));

      if (acquisition.Source == CharacterLanguageSource.Extra)
      {
        ascendancy = await _lineageRepository.LoadAscendancyAsync(character.LineageId, cancellationToken);
      }

      character.SetLanguage(language, acquisition, ascendancy, _context.ActorId);
    }
    else
    {
      if (existingAcquisition.Source != acquisition.Source)
      {
        throw new ImmutablePropertyException<CharacterLanguageSource>(character, existingAcquisition.Source, acquisition.Source, nameof(payload.Source));
      }
      if (existingAcquisition.Target != acquisition.Target)
      {
        throw new ImmutablePropertyException<string>(character, existingAcquisition.Target, acquisition.Target, nameof(payload.Target));
      }

      character.SetLanguage(languageId, acquisition, ascendancy, _context.ActorId);
    }

    await _characterRepository.SaveAsync(character, cancellationToken);

    CharacterModel model = await _characterQuerier.ReadAsync(character, cancellationToken);
    return new CreateOrReplaceCharacterLanguageResult(model, Created: existingAcquisition is null);
  }
}
