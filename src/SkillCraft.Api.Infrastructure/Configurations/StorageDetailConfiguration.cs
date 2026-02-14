using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class StorageDetailConfiguration : IEntityTypeConfiguration<StorageDetailEntity>
{
  public void Configure(EntityTypeBuilder<StorageDetailEntity> builder)
  {
    builder.ToTable(nameof(GameContext.StorageDetail), GameContext.Schema);
    builder.HasKey(x => x.StorageDetailId);

    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.EntityKind, x.EntityId });
    builder.HasIndex(x => new { x.WorldUid, x.EntityKind, x.EntityId });
    builder.HasIndex(x => x.Size);

    builder.Property(x => x.Key).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.EntityKind).HasMaxLength(16);

    builder.HasOne(x => x.Summary).WithMany(x => x.Detail)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
