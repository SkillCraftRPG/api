using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CasteConfiguration : AggregateConfiguration<CasteEntity>, IEntityTypeConfiguration<CasteEntity>
{
  public override void Configure(EntityTypeBuilder<CasteEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Castes), Schemas.Game);
    builder.HasKey(x => x.CasteId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.Skill });
    builder.HasIndex(x => new { x.WorldId, x.FeatureName });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(16).HasConversion(new EnumToStringConverter<Skill>());
    builder.Property(x => x.WealthRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.FeatureName).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Castes)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
