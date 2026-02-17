using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should throw ArgumentException when the script is from a different world.")]
  public void Given_ScriptFromDifferentWorld_When_SetScript_Then_ArgumentException()
  {
    Language language = new(_context.World, new Name("Celfique"));
    Script script = new(WorldId.NewId(), new Name("Elfique"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => language.SetScript(script));

    Assert.Equal("script", exception.ParamName);
  }
}
