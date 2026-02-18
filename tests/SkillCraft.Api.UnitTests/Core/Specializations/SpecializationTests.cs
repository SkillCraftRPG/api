namespace SkillCraft.Api.Core.Specializations;

[Trait(Traits.Category, Categories.Unit)]
public class SpecializationTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the tier is not valid.")]
  public void Given_InvalidTier_When_ctor_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Specialization(_context.World, new Tier(0), new Name("Éclaireur")));
    Assert.Equal("tier", exception.ParamName);
  }
}
