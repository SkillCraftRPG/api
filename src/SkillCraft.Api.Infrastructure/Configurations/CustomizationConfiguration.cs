using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CustomizationConfiguration : AggregateConfiguration<CustomizationEntity>, IEntityTypeConfiguration<CustomizationEntity>
{
  public override void Configure(EntityTypeBuilder<CustomizationEntity> builder)
  {
    builder.ToTable(nameof(GameContext.Worlds), GameContext.Schema);
    builder.HasKey(x => x.CustomizationId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Kind);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);

    builder.Property(x => x.Kind).HasMaxLength(10).HasConversion(new EnumToStringConverter<CustomizationKind>());
    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Customizations).OnDelete(DeleteBehavior.Restrict);
  }
}
