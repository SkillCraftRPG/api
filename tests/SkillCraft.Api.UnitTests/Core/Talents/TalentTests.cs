using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

[Trait(Traits.Category, Categories.Unit)]
public class TalentTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentException when the required talent is from a different world.")]
  public void Given_RequiredTalentFromDifferentWorld_When_SetRequiredTalent_Then_ArgumentException()
  {
    Talent talent = new(_context.World, new Tier(0), new Name("Talent"));
    Talent requiredTalent = new(WorldId.NewId(), new Tier(0), new Name("Required"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => talent.SetRequiredTalent(requiredTalent));

    Assert.Equal("requiredTalent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not valid.")]
  public void Given_InvalidSkill_When_SetSkill_Then_ArgumentOutOfRangeException()
  {
    Talent talent = new(_context.World, new Tier(0), new Name("Talent"));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => talent.Skill = (GameSkill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw InvalidTalentRequirementException when the required talent tier is greater than the requiring talent tier.")]
  public void Given_RequiredTalentWithHigherTier_When_SetRequiredTalent_Then_InvalidTalentRequirementException()
  {
    Talent acrobaties = new(_context.World, new Tier(0), new Name("Acrobaties"));
    Talent culbutes = new(_context.World, new Tier(1), new Name("Culbutes"));

    var exception = Assert.Throws<InvalidTalentRequirementException>(() => acrobaties.SetRequiredTalent(culbutes));

    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(acrobaties.EntityId, exception.RequiringTalentId);
    Assert.Equal(acrobaties.Tier.Value, exception.RequiringTalentTier);
    Assert.Equal(culbutes.EntityId, exception.RequiredTalentId);
    Assert.Equal(culbutes.Tier.Value, exception.RequiredTalentTier);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }
}
