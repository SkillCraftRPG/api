using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CharacterInventoryEvents : IEventHandler<CharacterInventoryAdded>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CharacterInventoryAdded>, CharacterInventoryEvents>();
  }

  private readonly GameContext _database;

  public CharacterInventoryEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(CharacterInventoryAdded @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      character.Update(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
