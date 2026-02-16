using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Core.Talents.Queries;

internal record ReadTalentQuery(Guid Id) : IQuery<TalentModel?>;

internal class ReadTalentQueryHandler : IQueryHandler<ReadTalentQuery, TalentModel?>
{
  private readonly ITalentQuerier _talentQuerier;

  public ReadTalentQueryHandler(ITalentQuerier talentQuerier)
  {
    _talentQuerier = talentQuerier;
  }

  public async Task<TalentModel?> HandleAsync(ReadTalentQuery query, CancellationToken cancellationToken)
  {
    return await _talentQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
