using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Languages;

public interface ILanguageRepository
{
  void Add(params Language[] languages);
  void Remove(Language language);
  void Update(Language language, LanguageUpdated record);

  Task<Language?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
}
