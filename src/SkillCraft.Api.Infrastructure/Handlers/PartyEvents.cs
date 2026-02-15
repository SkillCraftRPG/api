using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Core.Parties.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class PartyEvents : IEventHandler<PartyCreated>, IEventHandler<PartyDeleted>, IEventHandler<PartyUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<PartyCreated>, PartyEvents>();
    services.AddTransient<IEventHandler<PartyDeleted>, PartyEvents>();
    services.AddTransient<IEventHandler<PartyUpdated>, PartyEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<PartyEvents> _logger;

  public PartyEvents(GameContext game, ILogger<PartyEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(PartyCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      PartyEntity? party = await _game.Parties.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (party is not null)
      {
        throw new InvalidOperationException($"The party entity '{party}' was expected not to exist, but was found at version {party.Version}.");
      }

      PartyId partyId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == partyId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={partyId.WorldId}' was not found.");

      party = new PartyEntity(world, @event);
      _game.Parties.Add(party);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(PartyDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      PartyEntity? party = await _game.Parties.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (party is null)
      {
        throw new InvalidOperationException($"The party entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (party.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The party entity '{party}' was expected to be found at version {expectedVersion}, but was found at version {party.Version}.");
      }

      _game.Parties.Remove(party);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(PartyUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      PartyEntity? party = await _game.Parties.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (party is null)
      {
        throw new InvalidOperationException($"The party entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (party.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The party entity '{party}' was expected to be found at version {expectedVersion}, but was found at version {party.Version}.");
      }

      party.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
