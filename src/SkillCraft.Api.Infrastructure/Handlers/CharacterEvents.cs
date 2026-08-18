using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CharacterEvents : IEventHandler<CharacterCreated>,
  IEventHandler<CharacterDeleted>,
  IEventHandler<CharacterProfileChanged>,
  IEventHandler<CharacterRenamed>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CharacterCreated>, CharacterEvents>();
    services.AddTransient<IEventHandler<CharacterDeleted>, CharacterEvents>();
    services.AddTransient<IEventHandler<CharacterProfileChanged>, CharacterEvents>();
    services.AddTransient<IEventHandler<CharacterRenamed>, CharacterEvents>();
  }

  private readonly GameContext _database;

  public CharacterEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(CharacterCreated @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is null)
    {
      int lineageId = await _database.Lineages
        .Where(x => x.StreamId == @event.LineageId.Value)
        .Select(x => (int?)x.LineageId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The lineage entity 'StreamId={@event.LineageId}' was not found.");

      int casteId = await _database.Castes
        .Where(x => x.StreamId == @event.CasteId.Value)
        .Select(x => (int?)x.CasteId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The caste entity 'StreamId={@event.CasteId}' was not found.");

      int educationId = await _database.Educations
        .Where(x => x.StreamId == @event.EducationId.Value)
        .Select(x => (int?)x.EducationId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The education entity 'StreamId={@event.EducationId}' was not found.");

      HashSet<string> customizationIds = @event.CustomizationIds.Select(id => id.Value).ToHashSet();
      CustomizationEntity[] customizations = await _database.Customizations
        .Where(x => customizationIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      HashSet<string> languageIds = @event.LanguageIds.Select(id => id.Value).ToHashSet();
      LanguageEntity[] languages = await _database.Languages
        .Where(x => languageIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      HashSet<string> talentIds = @event.Talents.Select(acquisition => acquisition.Value.TalentId.Value).ToHashSet();
      TalentEntity[] talents = await _database.Talents
        .Where(x => talentIds.Contains(x.StreamId))
        .ToArrayAsync(cancellationToken);

      character = new CharacterEntity(lineageId, casteId, educationId, customizations, languages, talents, @event);

      _database.Characters.Add(character);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CharacterDeleted @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null)
    {
      _database.Characters.Remove(character);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CharacterProfileChanged @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      character.SetProfile(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CharacterRenamed @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      character.Rename(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
