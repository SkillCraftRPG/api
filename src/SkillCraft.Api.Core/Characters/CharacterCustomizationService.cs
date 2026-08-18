using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterCustomizationService
{
  Task<CharacterModel?> AddAsync(Guid characterId, Guid customizationId, CancellationToken cancellationToken = default);
  Task<CharacterModel?> RemoveAsync(Guid characterId, Guid customizationId, CancellationToken cancellationToken = default);
}

internal class CharacterCustomizationService : ICharacterCustomizationService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterCustomizationService, CharacterCustomizationService>();
    services.AddTransient<ICommandHandler<AddCharacterCustomizationCommand, CharacterModel?>, AddCharacterCustomizationCommandHandler>();
    services.AddTransient<ICommandHandler<RemoveCharacterCustomizationCommand, CharacterModel?>, RemoveCharacterCustomizationCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public CharacterCustomizationService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CharacterModel?> AddAsync(Guid characterId, Guid customizationId, CancellationToken cancellationToken)
  {
    AddCharacterCustomizationCommand command = new(characterId, customizationId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> RemoveAsync(Guid characterId, Guid customizationId, CancellationToken cancellationToken)
  {
    RemoveCharacterCustomizationCommand command = new(characterId, customizationId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
