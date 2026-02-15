using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class EducationEvents : IEventHandler<EducationCreated>, IEventHandler<EducationDeleted>, IEventHandler<EducationUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<EducationCreated>, EducationEvents>();
    services.AddTransient<IEventHandler<EducationDeleted>, EducationEvents>();
    services.AddTransient<IEventHandler<EducationUpdated>, EducationEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<EducationEvents> _logger;

  public EducationEvents(GameContext game, ILogger<EducationEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(EducationCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      EducationEntity? education = await _game.Educations.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (education is not null)
      {
        throw new InvalidOperationException($"The education entity '{education}' was expected not to exist, but was found at version {education.Version}.");
      }

      EducationId educationId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == educationId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={educationId.WorldId}' was not found.");

      education = new EducationEntity(world, @event);
      _game.Educations.Add(education);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(EducationDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      EducationEntity? education = await _game.Educations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (education is null)
      {
        throw new InvalidOperationException($"The education entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (education.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The education entity '{education}' was expected to be found at version {expectedVersion}, but was found at version {education.Version}.");
      }

      _game.Educations.Remove(education);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(EducationUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      EducationEntity? education = await _game.Educations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (education is null)
      {
        throw new InvalidOperationException($"The education entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (education.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The education entity '{education}' was expected to be found at version {expectedVersion}, but was found at version {education.Version}.");
      }

      education.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
