using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageConfiguration : IEntityTypeConfiguration<Lineage>
{
  public void Configure(EntityTypeBuilder<Lineage> builder)
  {
    builder.ToTable(nameof(GameContext.Lineages), Schemas.Game);
    builder.HasKey(x => x.LineageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.ParentId });
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.SizeCategory });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);
    builder.Property(x => x.SizeCategory).HasMaxLength(16).HasConversion(new EnumToStringConverter<SizeCategory>());
    builder.Property(x => x.HeightRoll).HasMaxLength(Constants.RollMaximumLength);
    builder.Property(x => x.Malnutrition).HasMaxLength(Constants.RollMaximumLength);
    builder.Property(x => x.Skinny).HasMaxLength(Constants.RollMaximumLength);
    builder.Property(x => x.NormalWeight).HasMaxLength(Constants.RollMaximumLength);
    builder.Property(x => x.Overweight).HasMaxLength(Constants.RollMaximumLength);
    builder.Property(x => x.Obese).HasMaxLength(Constants.RollMaximumLength);

    //builder.HasOne(x => x.World).WithMany(x => x.Lineages)
    //  .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
    //  .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Languages).WithMany(x => x.Lineages).UsingEntity<LineageLanguage>();
  }
}
