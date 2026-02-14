using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class StorageSummaryConfiguration : AggregateConfiguration<StorageSummaryEntity>, IEntityTypeConfiguration<StorageSummaryEntity>
{
  public override void Configure(EntityTypeBuilder<StorageSummaryEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.StorageSummary), GameContext.Schema);
    builder.HasKey(x => x.WorldId);

    builder.HasIndex(x => x.WorldUid).IsUnique();
    builder.HasIndex(x => x.AllocatedBytes);
    builder.HasIndex(x => x.UsedBytes);
    builder.HasIndex(x => x.RemainingBytes);

    builder.HasOne(x => x.World).WithOne(x => x.StorageSummary)
      .HasPrincipalKey<WorldEntity>(x => x.WorldId).HasForeignKey<StorageSummaryEntity>(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
