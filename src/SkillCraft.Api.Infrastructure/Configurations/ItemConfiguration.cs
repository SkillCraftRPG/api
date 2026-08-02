using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
  public void Configure(EntityTypeBuilder<Item> builder)
  {
    builder.ToTable(nameof(GameContext.Items), Schemas.Game);
    builder.HasKey(x => x.ItemId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.Price });
    builder.HasIndex(x => new { x.WorldId, x.Weight });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);

    //builder.HasOne(x => x.World).WithMany(x => x.Items)
    //  .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
    //  .OnDelete(DeleteBehavior.Restrict);
  }
}
