namespace SkillCraft.Api.Core.Educations.Events;

public class EducationDeleted : DeleteEvent
{
  public EducationDeleted() : base()
  {
  }

  public EducationDeleted(Education education, Guid userId) : base(education, userId)
  {
  }
}
