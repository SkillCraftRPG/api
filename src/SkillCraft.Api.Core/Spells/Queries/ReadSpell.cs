using Logitar.CQRS;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells.Queries;

internal record ReadSpellQuery(Guid Id) : IQuery<SpellModel?>;

internal class ReadSpellQueryHandler : IQueryHandler<ReadSpellQuery, SpellModel?>
{
  private readonly ISpellRepository _spellRepository;

  public ReadSpellQueryHandler(ISpellRepository spellRepository)
  {
    _spellRepository = spellRepository;
  }

  public async Task<SpellModel?> HandleAsync(ReadSpellQuery query, CancellationToken cancellationToken)
  {
    return await _spellRepository.ReadAsync(query.Id, cancellationToken);
  }
}
