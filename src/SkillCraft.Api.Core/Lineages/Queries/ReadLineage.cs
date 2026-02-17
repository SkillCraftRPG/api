using Logitar.CQRS;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Queries;

internal record ReadLineageQuery(Guid Id) : IQuery<LineageModel?>;

internal class ReadLineageQueryHandler : IQueryHandler<ReadLineageQuery, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;

  public ReadLineageQueryHandler(ILineageQuerier lineageQuerier)
  {
    _lineageQuerier = lineageQuerier;
  }

  public async Task<LineageModel?> HandleAsync(ReadLineageQuery query, CancellationToken cancellationToken)
  {
    return await _lineageQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
