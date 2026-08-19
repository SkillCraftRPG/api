using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CharacterLanguageEvents : IEventHandler<CharacterLanguageChanged>, IEventHandler<CharacterLanguageRemoved>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CharacterLanguageChanged>, CharacterLanguageEvents>();
    services.AddTransient<IEventHandler<CharacterLanguageRemoved>, CharacterLanguageEvents>();
  }

  private readonly GameContext _database;

  public CharacterLanguageEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(CharacterLanguageChanged @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters
      .Include(x => x.Languages).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      LanguageEntity language = await _database.Languages
        .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' was not found.");

      character.SetLanguage(language, @event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CharacterLanguageRemoved @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters
      .Include(x => x.Languages).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      character.RemoveLanguage(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
