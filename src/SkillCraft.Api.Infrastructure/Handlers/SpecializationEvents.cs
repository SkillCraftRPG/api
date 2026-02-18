using Logitar;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Core.Specializations.Events;
using SkillCraft.Api.Core.Talents;
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
      SpecializationEntity? specialization = await _game.Specializations
        .Include(x => x.OptionalTalents)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (specialization is null)
      {
        throw new InvalidOperationException($"The specialization entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (specialization.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The specialization entity '{specialization}' was expected to be found at version {expectedVersion}, but was found at version {specialization.Version}.");
      }

      IReadOnlyDictionary<TalentId, TalentEntity> talents = await GetTalentsAsync(@event, cancellationToken);
      TalentEntity? requiredTalent = @event.Requirements is not null && @event.Requirements.TalentId.HasValue
        ? talents[@event.Requirements.TalentId.Value]
        : null;

      specialization.Update(requiredTalent, @event);
      SetOptionalTalents(specialization, @event, talents);
      // TODO(fpion): Doctrine
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
  private async Task<IReadOnlyDictionary<TalentId, TalentEntity>> GetTalentsAsync(SpecializationUpdated @event, CancellationToken cancellationToken)
  {
    HashSet<string> streamIds = new(capacity: 1 + (@event.Options?.TalentIds.Count ?? 0)); // TODO(fpion): Doctrine
    if (@event.Requirements is not null && @event.Requirements.TalentId.HasValue)
    {
      streamIds.Add(@event.Requirements.TalentId.Value.Value);
    }
    if (@event.Options is not null)
    {
      streamIds.AddRange(@event.Options.TalentIds.Select(id => id.Value));
    }
    // TODO(fpion): Doctrine
    if (streamIds.Count < 1)
    {
      return new Dictionary<TalentId, TalentEntity>().AsReadOnly();
    }

    Dictionary<TalentId, TalentEntity> talents = await _game.Talents
      .Where(x => streamIds.Contains(x.StreamId))
      .ToDictionaryAsync(x => new TalentId(x.StreamId), x => x, cancellationToken);
    IEnumerable<TalentId> missingIds = streamIds.Select(id => new TalentId(id)).Except(talents.Keys);
    if (missingIds.Any())
    {
      StringBuilder message = new("The talent entities were not found.");
      message.AppendLine().AppendLine("StreamIds:");
      foreach (TalentId missingId in missingIds)
      {
        message.Append(" - ").Append(missingId).AppendLine();
      }
      throw new InvalidOperationException(message.ToString());
    }

    return talents.AsReadOnly();
  }
  private void SetOptionalTalents(SpecializationEntity specialization, SpecializationUpdated @event, IReadOnlyDictionary<TalentId, TalentEntity> talents)
  {
    if (@event.Options is not null)
    {
      HashSet<Guid> talentIds = @event.Options.TalentIds.Select(x => x.EntityId).ToHashSet();
      foreach (SpecializationOptionalTalentEntity optional in specialization.OptionalTalents)
      {
        if (!talentIds.Contains(optional.TalentUid))
        {
          _game.SpecializationOptionalTalents.Remove(optional);
        }
      }

      HashSet<TalentId> missingIds = new(capacity: @event.Options.TalentIds.Count);
      talentIds = specialization.OptionalTalents.Select(optional => optional.TalentUid).ToHashSet();
      foreach (TalentId talentId in @event.Options.TalentIds)
      {
        if (!talentIds.Contains(talentId.EntityId))
        {
          TalentEntity optionalTalent = talents[talentId];
          specialization.AddOptionalTalent(optionalTalent);
        }
      }
    }
  }
}
