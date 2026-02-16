using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class TalentEvents : IEventHandler<TalentCreated>, IEventHandler<TalentDeleted>, IEventHandler<TalentUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<TalentCreated>, TalentEvents>();
    services.AddTransient<IEventHandler<TalentDeleted>, TalentEvents>();
    services.AddTransient<IEventHandler<TalentUpdated>, TalentEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<TalentEvents> _logger;

  public TalentEvents(GameContext game, ILogger<TalentEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(TalentCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      TalentEntity? talent = await _game.Talents.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (talent is not null)
      {
        throw new InvalidOperationException($"The talent entity '{talent}' was expected not to exist, but was found at version {talent.Version}.");
      }

      TalentId talentId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == talentId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={talentId.WorldId}' was not found.");

      talent = new TalentEntity(world, @event);
      _game.Talents.Add(talent);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(TalentDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      TalentEntity? talent = await _game.Talents.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (talent is null)
      {
        throw new InvalidOperationException($"The talent entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (talent.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The talent entity '{talent}' was expected to be found at version {expectedVersion}, but was found at version {talent.Version}.");
      }

      _game.Talents.Remove(talent);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(TalentUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      TalentEntity? talent = await _game.Talents.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (talent is null)
      {
        throw new InvalidOperationException($"The talent entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (talent.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The talent entity '{talent}' was expected to be found at version {expectedVersion}, but was found at version {talent.Version}.");
      }

      TalentEntity? requiredTalent = null;
      if (@event.RequiredTalentId?.Value is not null)
      {
        requiredTalent = await _game.Talents.SingleOrDefaultAsync(x => x.StreamId == @event.RequiredTalentId.Value.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The talent entity 'StreamId={@event.RequiredTalentId.Value}' was not found.");
      }

      talent.Update(requiredTalent, @event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
