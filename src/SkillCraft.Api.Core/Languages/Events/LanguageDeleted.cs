namespace SkillCraft.Api.Core.Languages.Events;

public class LanguageDeleted : DeleteEvent
{
  public LanguageDeleted() : base()
  {
  }

  public LanguageDeleted(Language language, Guid userId) : base(language, userId)
  {
  }
}
