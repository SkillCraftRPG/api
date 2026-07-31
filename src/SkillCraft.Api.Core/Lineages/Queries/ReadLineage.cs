using Logitar.CQRS;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Core.Lineages.Queries;

internal record ReadLineageQuery(Guid Id) : IQuery<LineageModel?>;

internal class ReadLineageQueryHandler : IQueryHandler<ReadLineageQuery, LineageModel?>
{
  private readonly ILineageRepository _lineageRepository;

  public ReadLineageQueryHandler(ILineageRepository lineageRepository)
  {
    _lineageRepository = lineageRepository;
  }

  public async Task<LineageModel?> HandleAsync(ReadLineageQuery query, CancellationToken cancellationToken)
  {
    return await _lineageRepository.ReadAsync(query.Id, cancellationToken);
  }
}
