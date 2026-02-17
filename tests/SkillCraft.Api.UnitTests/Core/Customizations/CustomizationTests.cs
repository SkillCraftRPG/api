using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations;

[Trait(Traits.Category, Categories.Unit)]
public class CustomizationTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the kind is not valid.")]
  public void Given_InvalidKind_When_ctor_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Customization(_context.World, (CustomizationKind)(-1), new Name("Abruti")));
    Assert.Equal("kind", exception.ParamName);
  }
}
