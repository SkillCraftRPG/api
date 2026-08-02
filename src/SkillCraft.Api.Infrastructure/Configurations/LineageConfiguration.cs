using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageConfiguration : AggregateConfiguration<LineageEntity>, IEntityTypeConfiguration<LineageEntity>
{
  public override void Configure(EntityTypeBuilder<LineageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Lineages), Schemas.Game);
    builder.HasKey(x => x.LineageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.ParentId });
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.SizeCategory });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);
    builder.Property(x => x.SizeCategory).HasMaxLength(16).HasConversion(new EnumToStringConverter<SizeCategory>());
    builder.Property(x => x.HeightRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.Malnutrition).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.Skinny).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.NormalWeight).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.Overweight).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.Obese).HasMaxLength(Roll.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Lineages)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Parent).WithMany(x => x.Children)
      .HasForeignKey(x => x.ParentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
