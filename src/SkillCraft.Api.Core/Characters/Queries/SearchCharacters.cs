using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters.Queries;

internal record SearchCharactersQuery(SearchCharactersPayload Payload) : IQuery<SearchResults<CharacterModel>>;

internal class SearchCharactersQueryHandler : IQueryHandler<SearchCharactersQuery, SearchResults<CharacterModel>>
{
  private readonly ICharacterQuerier _characterQuerier;

  public SearchCharactersQueryHandler(ICharacterQuerier characterQuerier)
  {
    _characterQuerier = characterQuerier;
  }

  public async Task<SearchResults<CharacterModel>> HandleAsync(SearchCharactersQuery query, CancellationToken cancellationToken)
  {
    return await _characterQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
