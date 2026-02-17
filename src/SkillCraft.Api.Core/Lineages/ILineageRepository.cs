namespace SkillCraft.Api.Core.Lineages;

public interface ILineageRepository
{
  Task<Lineage?> LoadAsync(LineageId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Lineage>> LoadAsync(IEnumerable<LineageId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Lineage lineage, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken = default);
}
