using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Spells.Events;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells;

public interface ISpellRepository
{
  void Add(params Spell[] spells);
  void Remove(Spell spell);
  void Update(Spell spell, SpellUpdated record);

  Task<Spell?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SpellModel> ReadAsync(Spell spell, CancellationToken cancellationToken = default);
  Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken = default);
}
