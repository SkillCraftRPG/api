using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Languages), Schemas.Game);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.ScriptId });
    builder.HasIndex(x => new { x.WorldId, x.Summary });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Languages)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Script).WithMany(x => x.Languages)
      .HasForeignKey(x => x.ScriptId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
