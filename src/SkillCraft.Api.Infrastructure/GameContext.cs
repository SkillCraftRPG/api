using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  internal DbSet<Customization> Customizations => Set<Customization>();
  internal DbSet<HistoryRecord> History => Set<HistoryRecord>();
  internal DbSet<World> Worlds => Set<World>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
