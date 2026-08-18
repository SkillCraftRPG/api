using Logitar.CQRS;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record RemoveCharacterLanguageCommand(Guid CharacterId, Guid LanguageId) : ICommand<CharacterModel?>;

internal class RemoveCharacterLanguageCommandHandler : ICommandHandler<RemoveCharacterLanguageCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public RemoveCharacterLanguageCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IContext context,
    IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _context = context;
    _permissionService = permissionService;
  }

  public async Task<CharacterModel?> HandleAsync(RemoveCharacterLanguageCommand command, CancellationToken cancellationToken)
  {
    CharacterId characterId = new(_context.WorldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, character, cancellationToken);

    LanguageId languageId = new(character.WorldId, command.LanguageId);
    if (!character.HasLanguage(languageId))
    {
      return null;
    }
    character.RemoveLanguage(languageId, _context.ActorId);

    await _characterRepository.SaveAsync(character, cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
