using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterLanguageService
{
  Task<CharacterModel?> RemoveAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken = default);
}

internal class CharacterLanguageService : ICharacterLanguageService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterLanguageService, CharacterLanguageService>();
    services.AddTransient<ICommandHandler<RemoveCharacterLanguageCommand, CharacterModel?>, RemoveCharacterLanguageCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public CharacterLanguageService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CharacterModel?> RemoveAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken)
  {
    RemoveCharacterLanguageCommand command = new(characterId, languageId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
