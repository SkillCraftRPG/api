using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CharacterCustomizationConfiguration : IEntityTypeConfiguration<CharacterCustomizationEntity>
{
  public void Configure(EntityTypeBuilder<CharacterCustomizationEntity> builder)
  {
    builder.ToTable(nameof(GameContext.CharacterCustomizations), Schemas.Game);
    builder.HasKey(x => new { x.CharacterId, x.CustomizationId });

    builder.HasIndex(x => x.CustomizationId);

    builder.HasOne(x => x.Character).WithMany(x => x.Customizations)
      .HasForeignKey(x => x.CharacterId).HasPrincipalKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Customization).WithMany(x => x.Characters)
      .HasForeignKey(x => x.CustomizationId).HasPrincipalKey(x => x.CustomizationId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
