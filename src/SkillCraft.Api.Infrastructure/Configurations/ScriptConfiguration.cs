using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class ScriptConfiguration : AggregateConfiguration<ScriptEntity>, IEntityTypeConfiguration<ScriptEntity>
{
  public override void Configure(EntityTypeBuilder<ScriptEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Scripts), GameContext.Schema);
    builder.HasKey(x => x.ScriptId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Scripts).OnDelete(DeleteBehavior.Restrict);
  }
}
