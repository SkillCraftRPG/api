using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterLanguageService
{
  Task<CreateOrReplaceCharacterLanguageResult?> CreateOrReplaceAsync(
    Guid characterId,
    Guid languageId,
    CreateOrReplaceCharacterLanguagePayload payload,
    CancellationToken cancellationToken = default);
  Task<CharacterModel?> RemoveAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken = default);
}

internal class CharacterLanguageService : ICharacterLanguageService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterLanguageService, CharacterLanguageService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceCharacterLanguageCommand, CreateOrReplaceCharacterLanguageResult?>, CreateOrReplaceCharacterLanguageCommandHandler>();
    services.AddTransient<ICommandHandler<RemoveCharacterLanguageCommand, CharacterModel?>, RemoveCharacterLanguageCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public CharacterLanguageService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CreateOrReplaceCharacterLanguageResult?> CreateOrReplaceAsync(
    Guid characterId,
    Guid languageId,
    CreateOrReplaceCharacterLanguagePayload payload,
    CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterLanguageCommand command = new(characterId, languageId, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> RemoveAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken)
  {
    RemoveCharacterLanguageCommand command = new(characterId, languageId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
