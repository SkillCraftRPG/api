using Logitar.CQRS;
using SkillCraft.Api.Core.Castes.Models;

namespace SkillCraft.Api.Core.Castes.Queries;

internal record ReadCasteQuery(Guid Id) : IQuery<CasteModel?>;

internal class ReadCasteQueryHandler : IQueryHandler<ReadCasteQuery, CasteModel?>
{
  private readonly ICasteRepository _casteRepository;

  public ReadCasteQueryHandler(ICasteRepository casteRepository)
  {
    _casteRepository = casteRepository;
  }

  public async Task<CasteModel?> HandleAsync(ReadCasteQuery query, CancellationToken cancellationToken)
  {
    return await _casteRepository.ReadAsync(query.Id, cancellationToken);
  }
}
