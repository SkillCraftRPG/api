using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CharacterModifierEvents : IEventHandler<CharacterModifierChanged>, IEventHandler<CharacterModifierRemoved>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CharacterModifierChanged>, CharacterModifierEvents>();
    services.AddTransient<IEventHandler<CharacterModifierRemoved>, CharacterModifierEvents>();
  }

  private readonly GameContext _database;

  public CharacterModifierEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(CharacterModifierChanged @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters
      .Include(x => x.Modifiers)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null)
    {
      character.SetModifier(@event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CharacterModifierRemoved @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters
      .Include(x => x.Modifiers)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null)
    {
      character.RemoveModifier(@event);
      //CharacterModifierEntity? modifier = character.RemoveModifier(@event);
      //if (modifier is not null)
      //{
      //  _database.CharacterModifiers.Remove(modifier);
      //}

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
