namespace SkillCraft.Api.Core.Customizations.Events;

public class CustomizationDeleted : DeleteEvent
{
  public CustomizationDeleted() : base()
  {
  }

  public CustomizationDeleted(Customization customization, Guid userId) : base(customization, userId)
  {
  }
}
