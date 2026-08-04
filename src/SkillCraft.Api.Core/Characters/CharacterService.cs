using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Commands;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Characters.Queries;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterService
{
  Task<CharacterModel> CreateAsync(CreateCharacterPayload payload, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<CharacterModel>> SearchAsync(SearchCharactersPayload payload, CancellationToken cancellationToken = default);
}

internal class CharacterService : ICharacterService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICharacterService, CharacterService>();
    services.AddTransient<ICommandHandler<CreateCharacterCommand, CharacterModel>, CreateCharacterCommandHandler>();
    services.AddTransient<IQueryHandler<ReadCharacterQuery, CharacterModel?>, ReadCharacterQueryHandler>();
    services.AddTransient<IQueryHandler<SearchCharactersQuery, SearchResults<CharacterModel>>, SearchCharactersQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public CharacterService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CharacterModel> CreateAsync(CreateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CreateCharacterCommand command = new(payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadCharacterQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<CharacterModel>> SearchAsync(SearchCharactersPayload payload, CancellationToken cancellationToken)
  {
    SearchCharactersQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
