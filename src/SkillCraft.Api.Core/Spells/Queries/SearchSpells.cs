using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells.Queries;

internal record SearchSpellsQuery(SearchSpellsPayload Payload) : IQuery<SearchResults<SpellModel>>;

internal class SearchSpellsQueryHandler : IQueryHandler<SearchSpellsQuery, SearchResults<SpellModel>>
{
  private readonly ISpellRepository _spellRepository;

  public SearchSpellsQueryHandler(ISpellRepository spellRepository)
  {
    _spellRepository = spellRepository;
  }

  public async Task<SearchResults<SpellModel>> HandleAsync(SearchSpellsQuery query, CancellationToken cancellationToken)
  {
    return await _spellRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
