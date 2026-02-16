using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class ScriptEvents : IEventHandler<ScriptCreated>, IEventHandler<ScriptDeleted>, IEventHandler<ScriptUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<ScriptCreated>, ScriptEvents>();
    services.AddTransient<IEventHandler<ScriptDeleted>, ScriptEvents>();
    services.AddTransient<IEventHandler<ScriptUpdated>, ScriptEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<ScriptEvents> _logger;

  public ScriptEvents(GameContext game, ILogger<ScriptEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(ScriptCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      ScriptEntity? script = await _game.Scripts.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (script is not null)
      {
        throw new InvalidOperationException($"The script entity '{script}' was expected not to exist, but was found at version {script.Version}.");
      }

      ScriptId scriptId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == scriptId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={scriptId.WorldId}' was not found.");

      script = new ScriptEntity(world, @event);
      _game.Scripts.Add(script);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(ScriptDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      ScriptEntity? script = await _game.Scripts.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (script is null)
      {
        throw new InvalidOperationException($"The script entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (script.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The script entity '{script}' was expected to be found at version {expectedVersion}, but was found at version {script.Version}.");
      }

      _game.Scripts.Remove(script);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(ScriptUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      ScriptEntity? script = await _game.Scripts.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (script is null)
      {
        throw new InvalidOperationException($"The script entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (script.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The script entity '{script}' was expected to be found at version {expectedVersion}, but was found at version {script.Version}.");
      }

      script.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
