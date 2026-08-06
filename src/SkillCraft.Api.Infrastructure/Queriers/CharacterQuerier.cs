using Krakenar.Contracts.Search;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CharacterQuerier : ICharacterQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CharacterQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public async Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken)
  {
    return await ReadAsync(character.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'StreamId={character.Id}' was not found.");
  }
  public async Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }
  public async Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }

  public async Task<SearchResults<CharacterModel>> SearchAsync(SearchCharactersPayload payload, CancellationToken cancellationToken)
  {
    return new SearchResults<CharacterModel>(); // TODO(fpion): implement
  }
}
