using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Talents;

public interface ITalentService
{
  Task<CreateOrReplaceTalentResult> CreateOrReplaceAsync(CreateOrReplaceTalentPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<TalentModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken = default);
  Task<TalentModel?> UpdateAsync(Guid id, UpdateTalentPayload payload, CancellationToken cancellationToken = default);
}
