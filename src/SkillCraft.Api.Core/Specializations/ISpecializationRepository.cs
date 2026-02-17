namespace SkillCraft.Api.Core.Specializations;

public interface ISpecializationRepository
{
  Task<Specialization?> LoadAsync(SpecializationId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Specialization>> LoadAsync(IEnumerable<SpecializationId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Specialization specialization, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Specialization> specializations, CancellationToken cancellationToken = default);
}
