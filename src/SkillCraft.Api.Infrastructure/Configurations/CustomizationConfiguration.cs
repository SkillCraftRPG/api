using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CustomizationConfiguration : IEntityTypeConfiguration<Customization>
{
  public void Configure(EntityTypeBuilder<Customization> builder)
  {
    builder.ToTable(nameof(GameContext.Customizations), Schemas.Game);
    builder.HasKey(x => x.CustomizationId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Kind });
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Kind).HasMaxLength(10).HasConversion(new EnumToStringConverter<CustomizationKind>());
    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength).IsRequired();
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Customizations)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
