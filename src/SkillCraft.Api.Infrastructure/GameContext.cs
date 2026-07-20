using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  internal DbSet<Caste> Castes => Set<Caste>();
  internal DbSet<Customization> Customizations => Set<Customization>();
  internal DbSet<Education> Educations => Set<Education>();
  internal DbSet<HistoryRecord> History => Set<HistoryRecord>();
  internal DbSet<Language> Languages => Set<Language>();
  internal DbSet<Script> Scripts => Set<Script>();
  internal DbSet<Talent> Talents => Set<Talent>();
  internal DbSet<World> Worlds => Set<World>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
