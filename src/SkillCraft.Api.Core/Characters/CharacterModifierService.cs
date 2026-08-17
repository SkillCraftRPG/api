using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterModifierService
{
  Task<CreateOrReplaceCharacterModifierResult?> CreateOrReplaceAsync(
    Guid characterId,
    CreateOrReplaceCharacterModifierPayload payload,
    Guid? modifierId = null,
    CancellationToken cancellationToken = default);
  Task<CharacterModel?> RemoveAsync(Guid characterId, Guid modifierId, CancellationToken cancellationToken = default);
}

internal class CharacterModifierService : ICharacterModifierService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterModifierService, CharacterModifierService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceCharacterModifierCommand, CreateOrReplaceCharacterModifierResult?>, CreateOrReplaceCharacterModifierCommandHandler>();
    services.AddTransient<ICommandHandler<RemoveCharacterModifierCommand, CharacterModel?>, RemoveCharacterModifierCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public CharacterModifierService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CreateOrReplaceCharacterModifierResult?> CreateOrReplaceAsync(
    Guid characterId,
    CreateOrReplaceCharacterModifierPayload payload,
    Guid? modifierId,
    CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterModifierCommand command = new(characterId, payload, modifierId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> RemoveAsync(Guid characterId, Guid modifierId, CancellationToken cancellationToken)
  {
    RemoveCharacterModifierCommand command = new(characterId, modifierId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
