using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class LineageEvents : IEventHandler<LineageCreated>, IEventHandler<LineageDeleted>, IEventHandler<LineageUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<LineageCreated>, LineageEvents>();
    services.AddTransient<IEventHandler<LineageDeleted>, LineageEvents>();
    services.AddTransient<IEventHandler<LineageUpdated>, LineageEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<LineageEvents> _logger;

  public LineageEvents(GameContext game, ILogger<LineageEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(LineageCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      LineageEntity? lineage = await _game.Lineages.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (lineage is not null)
      {
        throw new InvalidOperationException($"The lineage entity '{lineage}' was expected not to exist, but was found at version {lineage.Version}.");
      }

      LineageId lineageId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == lineageId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={lineageId.WorldId}' was not found.");

      lineage = new LineageEntity(world, @event);
      _game.Lineages.Add(lineage);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(LineageDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      LineageEntity? lineage = await _game.Lineages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (lineage is null)
      {
        throw new InvalidOperationException($"The lineage entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (lineage.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The lineage entity '{lineage}' was expected to be found at version {expectedVersion}, but was found at version {lineage.Version}.");
      }

      _game.Lineages.Remove(lineage);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(LineageUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      LineageEntity? lineage = await _game.Lineages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (lineage is null)
      {
        throw new InvalidOperationException($"The lineage entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (lineage.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The lineage entity '{lineage}' was expected to be found at version {expectedVersion}, but was found at version {lineage.Version}.");
      }

      lineage.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
