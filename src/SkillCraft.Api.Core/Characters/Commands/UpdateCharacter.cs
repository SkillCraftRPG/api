using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record UpdateCharacterCommand(Guid Id, UpdateCharacterPayload Payload) : ICommand<CharacterModel?>;

internal class UpdateCharacterCommandHandler : ICommandHandler<UpdateCharacterCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public UpdateCharacterCommandHandler(
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

  public async Task<CharacterModel?> HandleAsync(UpdateCharacterCommand command, CancellationToken cancellationToken)
  {
    UpdateCharacterPayload payload = command.Payload;
    payload.Validate();

    CharacterId characterId = new(_context.WorldId, command.Id);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, character, cancellationToken);

    ActorId? actorId = _context.ActorId;

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      character.Rename(name, actorId);
    }

    if (payload.DominantHand is not null || payload.Appearance is not null || payload.Alignment is not null || payload.Personality is not null || payload.Background is not null)
    {
      character.SetProfile(
        payload.DominantHand is null ? character.DominantHand : payload.DominantHand.Value,
        payload.Appearance is null ? character.Appearance : new CharacterAppearance(payload.Appearance),
        payload.Alignment is null ? character.Alignment : payload.Alignment.Value,
        payload.Personality is null ? character.Personality : new CharacterPersonality(payload.Personality),
        payload.Background is null ? character.Background : Background.TryCreate(payload.Background.Value),
        actorId);
    }

    await _characterRepository.SaveAsync(character, cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
