using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class SpellConfiguration : AggregateConfiguration<SpellEntity>, IEntityTypeConfiguration<SpellEntity>
{
  public override void Configure(EntityTypeBuilder<SpellEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Spells), Schemas.Game);
    builder.HasKey(x => x.SpellId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Tier });
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Spells)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
