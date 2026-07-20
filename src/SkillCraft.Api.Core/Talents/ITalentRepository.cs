using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Talents;

public interface ITalentRepository
{
  void Add(params Talent[] talents);
  void Remove(Talent talent);
  void Update(Talent talent, TalentUpdated record);

  Task<Talent?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken = default);
}
