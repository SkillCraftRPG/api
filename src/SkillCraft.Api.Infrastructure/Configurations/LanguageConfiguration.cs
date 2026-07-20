using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
  public void Configure(EntityTypeBuilder<Language> builder)
  {
    builder.ToTable(nameof(GameContext.Languages), Schemas.Game);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.ScriptId });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength).IsRequired();

    builder.HasOne(x => x.World).WithMany(x => x.Languages)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Script).WithMany(x => x.Languages).OnDelete(DeleteBehavior.Restrict);
  }
}
