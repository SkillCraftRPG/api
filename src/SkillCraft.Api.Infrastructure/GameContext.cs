using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  internal const string Schema = "Game";

  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  internal DbSet<CustomizationEntity> Customizations => Set<CustomizationEntity>();
  internal DbSet<StorageSummaryEntity> StorageSummary => Set<StorageSummaryEntity>();
  internal DbSet<WorldEntity> Worlds => Set<WorldEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
