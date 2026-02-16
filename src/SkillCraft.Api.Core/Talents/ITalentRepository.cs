namespace SkillCraft.Api.Core.Talents;

public interface ITalentRepository
{
  Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Talent>> LoadAsync(IEnumerable<TalentId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Talent talent, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken = default);
}
