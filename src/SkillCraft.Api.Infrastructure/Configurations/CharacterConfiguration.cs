using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CharacterConfiguration : AggregateConfiguration<CharacterEntity>, IEntityTypeConfiguration<CharacterEntity>
{
  public override void Configure(EntityTypeBuilder<CharacterEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Characters), Schemas.Game);
    builder.HasKey(x => x.CharacterId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.LineageId });
    builder.HasIndex(x => new { x.WorldId, x.CasteId });
    builder.HasIndex(x => new { x.WorldId, x.EducationId });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.DominantHand).HasMaxLength(8).HasConversion(new EnumToStringConverter<DominantHand>());
    builder.Property(x => x.Skin).HasMaxLength(CharacterAppearance.MaximumLength);
    builder.Property(x => x.Eyes).HasMaxLength(CharacterAppearance.MaximumLength);
    builder.Property(x => x.Hair).HasMaxLength(CharacterAppearance.MaximumLength);
    builder.Property(x => x.Alignment).HasMaxLength(16).HasConversion(new EnumToStringConverter<Alignment>());

    builder.HasOne(x => x.World).WithMany(x => x.Characters)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Lineage).WithMany(x => x.Characters)
      .HasForeignKey(x => x.LineageId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Caste).WithMany(x => x.Characters)
      .HasForeignKey(x => x.CasteId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Education).WithMany(x => x.Characters)
      .HasForeignKey(x => x.EducationId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
