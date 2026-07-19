using Microsoft.EntityFrameworkCore;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
