using Logitar.CQRS;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Core.Scripts.Queries;

internal record ReadScriptQuery(Guid Id) : IQuery<ScriptModel?>;

internal class ReadScriptQueryHandler : IQueryHandler<ReadScriptQuery, ScriptModel?>
{
  private readonly IScriptRepository _scriptRepository;

  public ReadScriptQueryHandler(IScriptRepository scriptRepository)
  {
    _scriptRepository = scriptRepository;
  }

  public async Task<ScriptModel?> HandleAsync(ReadScriptQuery query, CancellationToken cancellationToken)
  {
    return await _scriptRepository.ReadAsync(query.Id, cancellationToken);
  }
}
