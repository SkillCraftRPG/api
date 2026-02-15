using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CasteConfiguration : AggregateConfiguration<CasteEntity>, IEntityTypeConfiguration<CasteEntity>
{
  public override void Configure(EntityTypeBuilder<CasteEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Castes), GameContext.Schema);
    builder.HasKey(x => x.CasteId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Castes).OnDelete(DeleteBehavior.Restrict);
  }
}
