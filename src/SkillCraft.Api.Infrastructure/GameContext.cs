using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Infrastructure.Configurations;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  internal DbSet<CasteEntity> Castes => Set<CasteEntity>();
  internal DbSet<CustomizationEntity> Customizations => Set<CustomizationEntity>();
  internal DbSet<EducationEntity> Educations => Set<EducationEntity>();
  internal DbSet<HistoryRecord> History => Set<HistoryRecord>();
  internal DbSet<Item> Items => Set<Item>();
  internal DbSet<Language> Languages => Set<Language>();
  internal DbSet<Lineage> Lineages => Set<Lineage>();
  internal DbSet<LineageFeature> LineageFeatures => Set<LineageFeature>();
  internal DbSet<LineageLanguage> LineageLanguages => Set<LineageLanguage>();
  internal DbSet<ScriptEntity> Scripts => Set<ScriptEntity>();
  internal DbSet<SpellEntity> Spells => Set<SpellEntity>();
  internal DbSet<TalentEntity> Talents => Set<TalentEntity>();
  internal DbSet<WorldEntity> Worlds => Set<WorldEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
