using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations.Events;

public class EducationUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? HtmlContent { get; set; }

  public Change<Skill?>? Skill { get; set; }
  public Change<int?>? WealthMultiplier { get; set; }
  public Change<Feature>? Feature { get; set; }

  public EducationUpdated() : base()
  {
  }

  public EducationUpdated(Education education) : base(education)
  {
  }
}
