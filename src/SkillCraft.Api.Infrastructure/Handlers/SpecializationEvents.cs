using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Core.Specializations.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class SpecializationEvents : IEventHandler<SpecializationCreated>, IEventHandler<SpecializationDeleted>, IEventHandler<SpecializationUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<SpecializationCreated>, SpecializationEvents>();
    services.AddTransient<IEventHandler<SpecializationDeleted>, SpecializationEvents>();
    services.AddTransient<IEventHandler<SpecializationUpdated>, SpecializationEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<SpecializationEvents> _logger;

  public SpecializationEvents(GameContext game, ILogger<SpecializationEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(SpecializationCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      SpecializationEntity? specialization = await _game.Specializations.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (specialization is not null)
      {
        throw new InvalidOperationException($"The specialization entity '{specialization}' was expected not to exist, but was found at version {specialization.Version}.");
      }

      SpecializationId specializationId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == specializationId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={specializationId.WorldId}' was not found.");

      specialization = new SpecializationEntity(world, @event);
      _game.Specializations.Add(specialization);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(SpecializationDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      SpecializationEntity? specialization = await _game.Specializations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (specialization is null)
      {
        throw new InvalidOperationException($"The specialization entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (specialization.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The specialization entity '{specialization}' was expected to be found at version {expectedVersion}, but was found at version {specialization.Version}.");
      }

      _game.Specializations.Remove(specialization);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(SpecializationUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      SpecializationEntity? specialization = await _game.Specializations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (specialization is null)
      {
        throw new InvalidOperationException($"The specialization entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (specialization.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The specialization entity '{specialization}' was expected to be found at version {expectedVersion}, but was found at version {specialization.Version}.");
      }

      specialization.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
