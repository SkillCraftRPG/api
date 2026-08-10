using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class CharacterTalentConfiguration : IEntityTypeConfiguration<CharacterTalentEntity>
{
  public void Configure(EntityTypeBuilder<CharacterTalentEntity> builder)
  {
    builder.ToTable(nameof(GameContext.CharacterTalents), Schemas.Game);
    builder.HasKey(x => x.CharacterTalentId);

    builder.HasIndex(x => new { x.CharacterId, x.Id }).IsUnique();
    builder.HasIndex(x => x.TalentId);

    builder.Property(x => x.Qualifier).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Talents)
      .HasForeignKey(x => x.CharacterId).HasPrincipalKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Talent).WithMany(x => x.Characters)
      .HasForeignKey(x => x.TalentId).HasPrincipalKey(x => x.TalentId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
