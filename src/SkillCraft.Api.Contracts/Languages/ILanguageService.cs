using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Languages;

public interface ILanguageService
{
  Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<LanguageModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
  Task<LanguageModel?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken = default);
}
