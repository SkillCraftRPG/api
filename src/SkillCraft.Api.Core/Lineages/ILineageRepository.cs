using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageRepository
{
  void Add(params Lineage[] lineages);
  void Add(LineageFeature feature);
  void Remove(Lineage lineage);
  void Remove(LineageFeature feature);
  void Update(Lineage lineage, LineageUpdated record);
  void Update(LineageFeature feature, LineageFeatureUpdated record);

  Task<Lineage?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken = default);
}
