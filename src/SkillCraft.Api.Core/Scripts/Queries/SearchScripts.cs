using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Core.Scripts.Queries;

internal record SearchScriptsQuery(SearchScriptsPayload Payload) : IQuery<SearchResults<ScriptModel>>;

internal class SearchScriptsQueryHandler : IQueryHandler<SearchScriptsQuery, SearchResults<ScriptModel>>
{
  private readonly IScriptRepository _scriptRepository;

  public SearchScriptsQueryHandler(IScriptRepository scriptRepository)
  {
    _scriptRepository = scriptRepository;
  }

  public async Task<SearchResults<ScriptModel>> HandleAsync(SearchScriptsQuery query, CancellationToken cancellationToken)
  {
    return await _scriptRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
