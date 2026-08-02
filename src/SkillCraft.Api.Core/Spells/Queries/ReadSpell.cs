using Logitar.CQRS;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells.Queries;

internal record ReadSpellQuery(Guid Id) : IQuery<SpellModel?>;

internal class ReadSpellQueryHandler : IQueryHandler<ReadSpellQuery, SpellModel?>
{
  private readonly ISpellQuerier _spellQuerier;

  public ReadSpellQueryHandler(ISpellQuerier spellQuerier)
  {
    _spellQuerier = spellQuerier;
  }

  public async Task<SpellModel?> HandleAsync(ReadSpellQuery query, CancellationToken cancellationToken)
  {
    return await _spellQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
