using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageConfiguration : AggregateConfiguration<LineageEntity>, IEntityTypeConfiguration<LineageEntity>
{
  public override void Configure(EntityTypeBuilder<LineageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Lineages), GameContext.Schema);
    builder.HasKey(x => x.LineageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.ParentId);
    builder.HasIndex(x => x.ParentUid);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);
    builder.Property(x => x.SizeCategory).HasMaxLength(10).HasConversion(new EnumToStringConverter<SizeCategory>());
    builder.Property(x => x.Height).HasMaxLength(Roll.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Lineages).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Parent).WithMany(x => x.Children)
      .HasPrincipalKey(x => x.LineageId).HasForeignKey(x => x.ParentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
