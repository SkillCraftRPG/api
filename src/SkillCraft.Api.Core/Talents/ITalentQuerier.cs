using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Core.Talents;

public interface ITalentQuerier
{
  Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken = default);
}
