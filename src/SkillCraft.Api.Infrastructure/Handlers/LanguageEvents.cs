using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class LanguageEvents : IEventHandler<LanguageCreated>, IEventHandler<LanguageDeleted>, IEventHandler<LanguageUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<LanguageCreated>, LanguageEvents>();
    services.AddTransient<IEventHandler<LanguageDeleted>, LanguageEvents>();
    services.AddTransient<IEventHandler<LanguageUpdated>, LanguageEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<LanguageEvents> _logger;

  public LanguageEvents(GameContext game, ILogger<LanguageEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(LanguageCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      LanguageEntity? language = await _game.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (language is not null)
      {
        throw new InvalidOperationException($"The language entity '{language}' was expected not to exist, but was found at version {language.Version}.");
      }

      LanguageId languageId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == languageId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={languageId.WorldId}' was not found.");

      language = new LanguageEntity(world, @event);
      _game.Languages.Add(language);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(LanguageDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      LanguageEntity? language = await _game.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (language is null)
      {
        throw new InvalidOperationException($"The language entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (language.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The language entity '{language}' was expected to be found at version {expectedVersion}, but was found at version {language.Version}.");
      }

      _game.Languages.Remove(language);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(LanguageUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      LanguageEntity? language = await _game.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (language is null)
      {
        throw new InvalidOperationException($"The language entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (language.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The language entity '{language}' was expected to be found at version {expectedVersion}, but was found at version {language.Version}.");
      }

      language.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
