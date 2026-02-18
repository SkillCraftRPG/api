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

      LineageEntity? parent = null;
      if (@event.ParentId is not null)
      {
        parent = await _game.Lineages.SingleOrDefaultAsync(x => x.StreamId == @event.ParentId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The lineage entity 'StreamId={@event.ParentId.Value}' was not found.");
      }

      lineage = new LineageEntity(world, parent, @event);
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
      LineageEntity? lineage = await _game.Lineages
        .Include(x => x.Languages)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (lineage is null)
      {
        throw new InvalidOperationException($"The lineage entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (lineage.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The lineage entity '{lineage}' was expected to be found at version {expectedVersion}, but was found at version {lineage.Version}.");
      }

      if (@event.Languages is not null)
      {
        IReadOnlyCollection<LanguageEntity> languages = await GetLanguagesAsync(@event.Languages, cancellationToken);

        HashSet<Guid> languageIds = @event.Languages.Ids.Select(id => id.EntityId).ToHashSet();
        foreach (LineageLanguageEntity entity in lineage.Languages)
        {
          if (!languageIds.Contains(entity.LanguageUid))
          {
            _game.LineageLanguages.Remove(entity);
          }
        }

        languageIds = lineage.Languages.Select(entity => entity.LanguageUid).ToHashSet();
        foreach (LanguageEntity language in languages)
        {
          if (!languageIds.Contains(language.Id))
          {
            lineage.AddLanguage(language);
          }
        }
      }

      lineage.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
  private async Task<IReadOnlyCollection<LanguageEntity>> GetLanguagesAsync(LanguageProficiencies proficiencies, CancellationToken cancellationToken)
  {
    HashSet<string> streamIds = proficiencies.Ids.Select(id => id.Value).ToHashSet();
    if (streamIds.Count < 1)
    {
      return [];
    }

    LanguageEntity[] languages = await _game.Languages.Where(x => streamIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);
    IEnumerable<string> missingIds = streamIds.Except(languages.Select(language => language.StreamId));
    if (missingIds.Any())
    {
      StringBuilder message = new("The language entities were not found.");
      message.AppendLine().AppendLine("StreamIds:");
      foreach (string missingId in missingIds)
      {
        message.Append(" - ").AppendLine(missingId);
      }
      throw new InvalidOperationException(message.ToString());
    }

    return languages.AsReadOnly();
  }
}
