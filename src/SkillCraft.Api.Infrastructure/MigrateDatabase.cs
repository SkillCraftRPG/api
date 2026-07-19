using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;

namespace SkillCraft.Api.Infrastructure;

public record MigrateDatabaseCommand : ICommand;

internal class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabaseCommand, Unit>
{
  private readonly GameContext _game;

  public MigrateDatabaseCommandHandler(GameContext game)
  {
    _game = game;
  }

  public async Task<Unit> HandleAsync(MigrateDatabaseCommand command, CancellationToken cancellationToken)
  {
    await _game.Database.MigrateAsync(cancellationToken);

    return Unit.Value;
  }
}
