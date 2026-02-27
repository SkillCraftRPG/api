using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Queries;

namespace SkillCraft.Api.Core.Characters;

internal class CharacterService : ICharacterService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterService, CharacterService>();
    services.AddTransient<ICommandHandler<CreateCharacterCommand, CharacterModel>, CreateCharacterCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteCharacterCommand, CharacterModel?>, DeleteCharacterCommandHandler>();
    services.AddTransient<IQueryHandler<ReadCharacterQuery, CharacterModel?>, ReadCharacterQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public CharacterService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CharacterModel> CreateOrReplaceAsync(CreateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CreateCharacterCommand command = new(payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteCharacterCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadCharacterQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
