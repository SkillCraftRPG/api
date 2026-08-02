using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace SkillCraft.Api.Infrastructure;

public record MigrateDatabaseCommand : ICommand;

internal class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabaseCommand, Unit>
{
  private readonly EventContext _events;
  private readonly GameContext _game;

  public MigrateDatabaseCommandHandler(EventContext events, GameContext game)
  {
    _events = events;
    _game = game;
  }

  public async Task<Unit> HandleAsync(MigrateDatabaseCommand command, CancellationToken cancellationToken)
  {
    await _events.Database.MigrateAsync(cancellationToken);
    await _game.Database.MigrateAsync(cancellationToken);

    return Unit.Value;
  }
}
