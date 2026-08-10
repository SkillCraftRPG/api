using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CharacterRepository : Repository, ICharacterRepository
{
  private readonly GameContext _database;

  public CharacterRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Character>> LoadAsync(IEnumerable<CharacterId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Character character, CancellationToken cancellationToken)
  {
    await base.SaveAsync(character, cancellationToken);

    await SynchronizeAsync(character, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken)
  {
    await base.SaveAsync(characters, cancellationToken);

    foreach (Character character in characters)
    {
      await SynchronizeAsync(character, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Character character, CancellationToken cancellationToken)
  {
    CharacterEntity? entity = await _database.Characters.SingleOrDefaultAsync(x => x.StreamId == character.Id.Value, cancellationToken);
    if (entity is null)
    {
      int lineageId = await _database.Lineages
        .Where(x => x.StreamId == character.LineageId.Value)
        .Select(x => (int?)x.LineageId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The lineage entity 'StreamId={character.LineageId}' was not found.");

      int casteId = await _database.Castes
        .Where(x => x.StreamId == character.CasteId.Value)
        .Select(x => (int?)x.CasteId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The caste entity 'StreamId={character.CasteId}' was not found.");

      int educationId = await _database.Educations
        .Where(x => x.StreamId == character.EducationId.Value)
        .Select(x => (int?)x.EducationId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The education entity 'StreamId={character.EducationId}' was not found.");

      HashSet<string> customizationIds = character.CustomizationIds.Select(id => id.Value).ToHashSet();
      CustomizationEntity[] customizations = await _database.Customizations
        .Where(x => customizationIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      HashSet<string> languageIds = character.LanguageIds.Select(id => id.Value).ToHashSet();
      LanguageEntity[] languages = await _database.Languages
        .Where(x => languageIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      HashSet<string> talentIds = character.Talents.Select(talent => talent.Value.TalentId.Value).ToHashSet();
      TalentEntity[] talents = await _database.Talents
        .Where(x => talentIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      entity = new CharacterEntity(character, lineageId, casteId, educationId, customizations, languages, talents);
      _database.Characters.Add(entity);
    }
    else
    {
      entity.Update(character);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
