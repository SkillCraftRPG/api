using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class ScriptConfiguration : IEntityTypeConfiguration<Script>
{
  public void Configure(EntityTypeBuilder<Script> builder)
  {
    builder.ToTable(nameof(GameContext.Scripts), Schemas.Game);
    builder.HasKey(x => x.ScriptId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength).IsRequired();
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Scripts)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
