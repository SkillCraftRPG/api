using Logitar.CQRS;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Core.Castes.Queries;

internal record ReadCasteQuery(Guid Id) : IQuery<CasteModel?>;

internal class ReadCasteQueryHandler : IQueryHandler<ReadCasteQuery, CasteModel?>
{
  private readonly ICasteQuerier _casteQuerier;

  public ReadCasteQueryHandler(ICasteQuerier casteQuerier)
  {
    _casteQuerier = casteQuerier;
  }

  public async Task<CasteModel?> HandleAsync(ReadCasteQuery query, CancellationToken cancellationToken)
  {
    return await _casteQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
