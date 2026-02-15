using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CasteEvents : IEventHandler<CasteCreated>, IEventHandler<CasteDeleted>, IEventHandler<CasteUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CasteCreated>, CasteEvents>();
    services.AddTransient<IEventHandler<CasteDeleted>, CasteEvents>();
    services.AddTransient<IEventHandler<CasteUpdated>, CasteEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<CasteEvents> _logger;

  public CasteEvents(GameContext game, ILogger<CasteEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(CasteCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      CasteEntity? caste = await _game.Castes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (caste is not null)
      {
        throw new InvalidOperationException($"The caste entity '{caste}' was expected not to exist, but was found at version {caste.Version}.");
      }

      CasteId casteId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == casteId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={casteId.WorldId}' was not found.");

      caste = new CasteEntity(world, @event);
      _game.Castes.Add(caste);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(CasteDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      CasteEntity? caste = await _game.Castes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (caste is null)
      {
        throw new InvalidOperationException($"The caste entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (caste.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The caste entity '{caste}' was expected to be found at version {expectedVersion}, but was found at version {caste.Version}.");
      }

      _game.Castes.Remove(caste);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(CasteUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      CasteEntity? caste = await _game.Castes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (caste is null)
      {
        throw new InvalidOperationException($"The caste entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (caste.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The caste entity '{caste}' was expected to be found at version {expectedVersion}, but was found at version {caste.Version}.");
      }

      caste.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
