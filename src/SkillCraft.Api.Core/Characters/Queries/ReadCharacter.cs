using Logitar.CQRS;
using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Core.Characters.Queries;

internal record ReadCharacterQuery(Guid Id) : IQuery<CharacterModel?>;

internal class ReadCharacterQueryHandler : IQueryHandler<ReadCharacterQuery, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;

  public ReadCharacterQueryHandler(ICharacterQuerier characterQuerier)
  {
    _characterQuerier = characterQuerier;
  }

  public async Task<CharacterModel?> HandleAsync(ReadCharacterQuery query, CancellationToken cancellationToken)
  {
    return await _characterQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
