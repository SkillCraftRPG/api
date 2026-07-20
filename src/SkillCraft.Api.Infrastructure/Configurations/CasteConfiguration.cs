using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CasteConfiguration : IEntityTypeConfiguration<Caste>
{
  public void Configure(EntityTypeBuilder<Caste> builder)
  {
    builder.ToTable(nameof(GameContext.Castes), Schemas.Game);
    builder.HasKey(x => x.CasteId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.Skill });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength).IsRequired();
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(16).HasConversion(new EnumToStringConverter<Skill>());
    builder.Property(x => x.WealthRoll).HasMaxLength(Constants.RollMaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Castes)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
