using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class PartyConfiguration : AggregateConfiguration<PartyEntity>, IEntityTypeConfiguration<PartyEntity>
{
  public override void Configure(EntityTypeBuilder<PartyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Parties), GameContext.Schema);
    builder.HasKey(x => x.PartyId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Parties).OnDelete(DeleteBehavior.Restrict);
  }
}
