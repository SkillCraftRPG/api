using Logitar.CQRS;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record DeleteCharacterCommand(Guid Id) : ICommand<CharacterModel?>;

internal class DeleteCharacterCommandHandler : ICommandHandler<DeleteCharacterCommand, CharacterModel?>
{
  private readonly IContext _context;
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteCharacterCommandHandler(
    IContext context,
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<CharacterModel?> HandleAsync(DeleteCharacterCommand command, CancellationToken cancellationToken)
  {
    CharacterId characterId = new(command.Id, _context.WorldId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, character, cancellationToken);
    CharacterModel model = await _characterQuerier.ReadAsync(character, cancellationToken);

    character.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      character,
      async () => await _characterRepository.SaveAsync(character, cancellationToken),
      cancellationToken);

    return model;
  }
}
