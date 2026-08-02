namespace SkillCraft.Api.Core.Spells;

public interface ISpellRepository
{
  Task<Spell?> LoadAsync(SpellId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Spell>> LoadAsync(IEnumerable<SpellId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Spell spell, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Spell> spells, CancellationToken cancellationToken = default);
}
