using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

public class GameContext : DbContext
{
  internal const string Schema = "Game";

  public GameContext(DbContextOptions<GameContext> options) : base(options)
  {
  }

  internal DbSet<CasteEntity> Castes => Set<CasteEntity>();
  internal DbSet<CustomizationEntity> Customizations => Set<CustomizationEntity>();
  internal DbSet<EducationEntity> Educations => Set<EducationEntity>();
  internal DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
  internal DbSet<LineageEntity> Lineages => Set<LineageEntity>();
  internal DbSet<LineageLanguageEntity> LineageLanguages => Set<LineageLanguageEntity>();
  internal DbSet<PartyEntity> Parties => Set<PartyEntity>();
  internal DbSet<ScriptEntity> Scripts => Set<ScriptEntity>();
  internal DbSet<StorageDetailEntity> StorageDetail => Set<StorageDetailEntity>();
  internal DbSet<StorageSummaryEntity> StorageSummary => Set<StorageSummaryEntity>();
  internal DbSet<TalentEntity> Talents => Set<TalentEntity>();
  internal DbSet<WorldEntity> Worlds => Set<WorldEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
