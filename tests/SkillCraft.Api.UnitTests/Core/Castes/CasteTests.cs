using SkillCraft.Api.Contracts;

namespace SkillCraft.Api.Core.Castes;

[Trait(Traits.Category, Categories.Unit)]
public class CasteTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not valid.")]
  public void Given_InvalidSkill_When_SetSkill_Then_ArgumentOutOfRangeException()
  {
    Caste caste = new(_context.World, new Name("Noble"));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => caste.Skill = (GameSkill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }
}
