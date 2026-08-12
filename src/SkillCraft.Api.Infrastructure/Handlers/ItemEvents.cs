using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class ItemEvents : IEventHandler<ItemCreated>,
  IEventHandler<ItemDeleted>,
  IEventHandler<ItemEdited>,
  IEventHandler<ItemRenamed>,
  IEventHandler<ItemRulesChanged>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<ItemCreated>, ItemEvents>();
    services.AddTransient<IEventHandler<ItemDeleted>, ItemEvents>();
    services.AddTransient<IEventHandler<ItemEdited>, ItemEvents>();
    services.AddTransient<IEventHandler<ItemRenamed>, ItemEvents>();
    services.AddTransient<IEventHandler<ItemRulesChanged>, ItemEvents>();
  }

  private readonly GameContext _database;

  public ItemEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(ItemCreated @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _database.Items.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null)
    {
      item = new ItemEntity(@event);

      _database.Items.Add(item);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(ItemDeleted @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is not null)
    {
      _database.Items.Remove(item);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(ItemEdited @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is not null && item.Version == (@event.Version - 1))
    {
      item.Edit(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(ItemRenamed @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is not null && item.Version == (@event.Version - 1))
    {
      item.Rename(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(ItemRulesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is not null && item.Version == (@event.Version - 1))
    {
      ItemEntity? replacement = null;
      if (@event.Charges is not null && @event.Charges.ReplacementId.HasValue)
      {
        replacement = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == @event.Charges.ReplacementId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.Charges.ReplacementId}' was not found.");
      }

      item.SetRules(replacement, @event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
