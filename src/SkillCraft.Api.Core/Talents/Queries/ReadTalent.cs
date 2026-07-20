using Logitar.CQRS;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Talents.Queries;

internal record ReadTalentQuery(Guid Id) : IQuery<TalentModel?>;

internal class ReadTalentQueryHandler : IQueryHandler<ReadTalentQuery, TalentModel?>
{
  private readonly ITalentRepository _talentRepository;

  public ReadTalentQueryHandler(ITalentRepository talentRepository)
  {
    _talentRepository = talentRepository;
  }

  public async Task<TalentModel?> HandleAsync(ReadTalentQuery query, CancellationToken cancellationToken)
  {
    return await _talentRepository.ReadAsync(query.Id, cancellationToken);
  }
}
