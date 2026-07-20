namespace SkillCraft.Api.Core.Customizations.Events;

public class CustomizationUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Description { get; set; }

  public CustomizationUpdated() : base()
  {
  }

  public CustomizationUpdated(Customization customization) : base(customization)
  {
  }
}
