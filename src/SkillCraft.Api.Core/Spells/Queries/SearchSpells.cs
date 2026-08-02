using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells.Queries;

internal record SearchSpellsQuery(SearchSpellsPayload Payload) : IQuery<SearchResults<SpellModel>>;

internal class SearchSpellsQueryHandler : IQueryHandler<SearchSpellsQuery, SearchResults<SpellModel>>
{
  private readonly ISpellQuerier _spellQuerier;

  public SearchSpellsQueryHandler(ISpellQuerier spellQuerier)
  {
    _spellQuerier = spellQuerier;
  }

  public async Task<SearchResults<SpellModel>> HandleAsync(SearchSpellsQuery query, CancellationToken cancellationToken)
  {
    return await _spellQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
