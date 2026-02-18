using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations;

[Trait(Traits.Category, Categories.Unit)]
public class SpecializationTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should handle Options changes correctly.")]
  public void Given_Changes_When_SetOptions_Then_HandledCorrectly()
  {
    Talent survie = new(_context.World, new Tier(0), new Name("Survie"));
    Talent survivalisme = new(_context.World, new Tier(1), new Name("Survivalisme"));
    Talent[] talents = [survie, survivalisme];
    string[] other = ["Option A", "Option B"];

    Specialization specialization = new(_context.World, new Tier(2), new Name("Chasseur"));
    specialization.SetOptions(talents, other);
    specialization.Update(_context.UserId);
    Assert.True(specialization.HasChanges);

    specialization.ClearChanges();
    Assert.False(specialization.HasChanges);

    specialization.SetOptions(talents, other);
    specialization.Update(_context.UserId);
    Assert.False(specialization.HasChanges);
  }

  [Fact(DisplayName = "It should handle Requirements changes correctly.")]
  public void Given_Changes_When_SetRequirements_Then_HandledCorrectly()
  {
    Talent survie = new(_context.World, new Tier(0), new Name("Survie"));
    string[] other = ["Être de tier 1 ou plus", "Avoir 3 rangs en Survie"];

    Specialization specialization = new(_context.World, new Tier(2), new Name("Chasseur"));
    specialization.SetRequirements(survie, other);
    specialization.Update(_context.UserId);
    Assert.True(specialization.HasChanges);

    specialization.ClearChanges();
    Assert.False(specialization.HasChanges);

    specialization.SetRequirements(survie, other);
    specialization.Update(_context.UserId);
    Assert.False(specialization.HasChanges);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an option talent is from another world.")]
  public void Given_OptionTalentFromAnotherWorld_When_SetOptions_Then_ArgumentException()
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    Talent talent = new(WorldId.NewId(), new Tier(0), new Name("Survie"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => specialization.SetOptions([talent], []));

    Assert.Equal("talents", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the required talent is from another world.")]
  public void Given_RequiredTalentFromAnotherWorld_When_SetRequirements_Then_ArgumentException()
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    Talent talent = new(WorldId.NewId(), new Tier(0), new Name("Survie"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => specialization.SetRequirements(talent, []));

    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the tier is not valid.")]
  public void Given_InvalidTier_When_ctor_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Specialization(_context.World, new Tier(0), new Name("Éclaireur")));
    Assert.Equal("tier", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw NotImplementedException when an optional talent has tier greater than or equal to the specialization tier.")]
  public void Given_OptionTalentWithTierGreaterOrEqual_When_SetOptions_Then_NotImplementedException()
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    Talent armesDeTir = new(_context.World, new Tier(1), new Name("Armes de tir"));
    Talent tirPrecis = new(_context.World, new Tier(2), new Name("Tir précis"));

    Assert.Throws<NotImplementedException>(() => specialization.SetOptions([armesDeTir, tirPrecis], []));
  }

  [Theory(DisplayName = "It should throw NotImplementedException when the required talent has tier greater than or equal to the specialization tier.")]
  [InlineData(1, "Armes de tir")]
  [InlineData(2, "Tir précis")]
  public void Given_RequiredTalentWithTierGreaterOrEqual_When_SetRequirements_Then_NotImplementedException(int tier, string name)
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    Talent talent = new(_context.World, new Tier(tier), new Name(name));

    Assert.Throws<NotImplementedException>(() => specialization.SetRequirements(talent, []));
  }
}
