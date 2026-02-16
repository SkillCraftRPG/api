using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Core.Scripts.Queries;

internal record SearchScriptsQuery(SearchScriptsPayload Payload) : IQuery<SearchResults<ScriptModel>>;

internal class SearchScriptsQueryHandler : IQueryHandler<SearchScriptsQuery, SearchResults<ScriptModel>>
{
  private readonly IScriptQuerier _scriptQuerier;

  public SearchScriptsQueryHandler(IScriptQuerier scriptQuerier)
  {
    _scriptQuerier = scriptQuerier;
  }

  public async Task<SearchResults<ScriptModel>> HandleAsync(SearchScriptsQuery query, CancellationToken cancellationToken)
  {
    return await _scriptQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
