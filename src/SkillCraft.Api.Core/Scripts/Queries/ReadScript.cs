using Logitar.CQRS;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Core.Scripts.Queries;

internal record ReadScriptQuery(Guid Id) : IQuery<ScriptModel?>;

internal class ReadScriptQueryHandler : IQueryHandler<ReadScriptQuery, ScriptModel?>
{
  private readonly IScriptQuerier _scriptQuerier;

  public ReadScriptQueryHandler(IScriptQuerier scriptQuerier)
  {
    _scriptQuerier = scriptQuerier;
  }

  public async Task<ScriptModel?> HandleAsync(ReadScriptQuery query, CancellationToken cancellationToken)
  {
    return await _scriptQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
