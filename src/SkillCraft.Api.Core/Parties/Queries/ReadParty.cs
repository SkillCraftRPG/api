using Logitar.CQRS;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Core.Parties.Queries;

internal record ReadPartyQuery(Guid Id) : IQuery<PartyModel?>;

internal class ReadPartyQueryHandler : IQueryHandler<ReadPartyQuery, PartyModel?>
{
  private readonly IPartyQuerier _partyQuerier;

  public ReadPartyQueryHandler(IPartyQuerier partyQuerier)
  {
    _partyQuerier = partyQuerier;
  }

  public async Task<PartyModel?> HandleAsync(ReadPartyQuery query, CancellationToken cancellationToken)
  {
    return await _partyQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
