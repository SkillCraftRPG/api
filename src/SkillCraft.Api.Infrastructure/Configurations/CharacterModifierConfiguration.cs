using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CharacterModifierConfiguration : IEntityTypeConfiguration<CharacterModifierEntity>
{
  public void Configure(EntityTypeBuilder<CharacterModifierEntity> builder)
  {
    builder.ToTable(nameof(GameContext.CharacterModifiers), Schemas.Game);
    builder.HasKey(x => x.CharacterModifierId);

    builder.HasIndex(x => new { x.CharacterId, x.Id }).IsUnique();

    builder.Property(x => x.Kind).HasMaxLength(16).HasConversion(new EnumToStringConverter<CharacterModifierKind>());
    builder.Property(x => x.Target).HasMaxLength(16);
    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Modifiers)
      .HasForeignKey(x => x.CharacterId).HasPrincipalKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
