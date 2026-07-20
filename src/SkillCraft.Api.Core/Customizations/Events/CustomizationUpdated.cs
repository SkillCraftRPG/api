namespace SkillCraft.Api.Core.Customizations.Events;

public class CustomizationUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? HtmlContent { get; set; }

  public CustomizationUpdated() : base()
  {
  }

  public CustomizationUpdated(Customization customization) : base(customization)
  {
  }
}
