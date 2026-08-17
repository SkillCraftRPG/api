using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Characters.Commands;

internal record CreateOrReplaceCharacterModifierCommand(Guid CharacterId, CreateOrReplaceCharacterModifierPayload Payload, Guid? ModifierId)
  : ICommand<CreateOrReplaceCharacterModifierResult?>;

internal class CreateOrReplaceCharacterModifierCommandHandler : ICommandHandler<CreateOrReplaceCharacterModifierCommand, CreateOrReplaceCharacterModifierResult?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceCharacterModifierCommandHandler(
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

  public async Task<CreateOrReplaceCharacterModifierResult?> HandleAsync(CreateOrReplaceCharacterModifierCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterModifierPayload payload = command.Payload;
    payload.Validate();

    CharacterId characterId = new(_context.WorldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, character, cancellationToken);

    ActorId? actorId = _context.ActorId;

    bool created = true;
    CharacterModifier? modifier = new(payload.Kind, payload.Target, payload.Value, Name.TryCreate(payload.Name), Notes.TryCreate(payload.Notes));
    if (command.ModifierId.HasValue)
    {
      CharacterModifier? existingModifier = character.TryGetModifier(command.ModifierId.Value);
      if (existingModifier is not null)
      {
        if (existingModifier.Kind != modifier.Kind)
        {
          throw new ImmutablePropertyException<CharacterModifierKind>(character, existingModifier.Kind, modifier.Kind, nameof(payload.Kind));
        }
        if (existingModifier.Target != modifier.Target)
        {
          throw new ImmutablePropertyException<string>(character, existingModifier.Target, modifier.Target, nameof(payload.Target));
        }
        created = false;
      }
      character.SetModifier(command.ModifierId.Value, modifier, actorId);
    }
    else
    {
      character.AddModifier(modifier, actorId);
    }

    await _characterRepository.SaveAsync(character, cancellationToken);

    CharacterModel model = await _characterQuerier.ReadAsync(character, cancellationToken);
    return new CreateOrReplaceCharacterModifierResult(model, created);
  }
}
