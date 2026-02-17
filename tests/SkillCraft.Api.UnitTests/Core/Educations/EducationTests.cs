using SkillCraft.Api.Contracts;

namespace SkillCraft.Api.Core.Educations;

[Trait(Traits.Category, Categories.Unit)]
public class EducationTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not valid.")]
  public void Given_InvalidSkill_When_SetSkill_Then_ArgumentOutOfRangeException()
  {
    Education education = new(_context.World, new Name("Rebelle"));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => education.Skill = (GameSkill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }
}
