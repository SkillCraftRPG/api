using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CharacterCustomizationEvents : IEventHandler<CharacterCustomizationAdded>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CharacterCustomizationAdded>, CharacterCustomizationEvents>();
  }

  private readonly GameContext _database;

  public CharacterCustomizationEvents(GameContext database)
  {
    _database = database;
  }

  public async Task HandleAsync(CharacterCustomizationAdded @event, CancellationToken cancellationToken)
  {
    CharacterEntity? character = await _database.Characters
      .Include(x => x.Customizations).ThenInclude(x => x.Customization)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (character is not null && character.Version == (@event.Version - 1))
    {
      CustomizationEntity customization = await _database.Customizations
        .SingleOrDefaultAsync(x => x.StreamId == @event.CustomizationId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The customization entity 'StreamId={@event.CustomizationId}' was not found.");

      character.AddCustomization(customization, @event);

      await _database.SaveChangesAsync(cancellationToken);
    }
  }
}
