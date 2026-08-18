using Logitar.CQRS;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record AddCharacterCustomizationCommand(Guid CharacterId, Guid CustomizationId) : ICommand<CharacterModel?>;

internal class AddCharacterCustomizationCommandHandler : ICommandHandler<AddCharacterCustomizationCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public AddCharacterCustomizationCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IContext context,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _context = context;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<CharacterModel?> HandleAsync(AddCharacterCustomizationCommand command, CancellationToken cancellationToken)
  {
    CharacterId characterId = new(_context.WorldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, character, cancellationToken);

    CustomizationId customizationId = new(character.WorldId, command.CustomizationId);
    if (!character.HasCustomization(customizationId))
    {
      Customization customization = await _customizationRepository.LoadAsync(customizationId, cancellationToken)
        ?? throw new CustomizationNotFoundException(customizationId, nameof(command.CustomizationId));

      character.AddCustomization(customization, _context.ActorId);

      await _characterRepository.SaveAsync(character, cancellationToken);
    }

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
