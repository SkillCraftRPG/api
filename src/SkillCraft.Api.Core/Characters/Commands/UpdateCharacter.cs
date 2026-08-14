using Logitar.CQRS;
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

    // TODO(fpion): implement

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
