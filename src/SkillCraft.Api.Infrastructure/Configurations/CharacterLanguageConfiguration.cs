using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CharacterLanguageConfiguration : IEntityTypeConfiguration<CharacterLanguageEntity>
{
  public void Configure(EntityTypeBuilder<CharacterLanguageEntity> builder)
  {
    builder.ToTable(nameof(GameContext.CharacterLanguages), Schemas.Game);
    builder.HasKey(x => new { x.CharacterId, x.LanguageId });

    builder.HasIndex(x => x.LanguageId);

    builder.Property(x => x.Source).HasMaxLength(16).HasConversion(new EnumToStringConverter<CharacterLanguageSource>());
    builder.Property(x => x.Target).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Languages)
      .HasForeignKey(x => x.CharacterId).HasPrincipalKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.Characters)
      .HasForeignKey(x => x.LanguageId).HasPrincipalKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
