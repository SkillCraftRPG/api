using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells;

public interface ISpellQuerier
{
  Task<SpellModel> ReadAsync(Spell spell, CancellationToken cancellationToken = default);
  Task<SpellModel?> ReadAsync(SpellId id, CancellationToken cancellationToken = default);
  Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken = default);
}
