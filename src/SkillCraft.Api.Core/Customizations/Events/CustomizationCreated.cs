namespace SkillCraft.Api.Core.Customizations.Events;

public class CustomizationCreated : CreateEvent
{
  public CustomizationKind Kind { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public CustomizationCreated() : base()
  {
  }

  public CustomizationCreated(Customization customization) : base(customization)
  {
    Kind = customization.Kind;

    Name = customization.Name;
    Summary = customization.Summary;
    HtmlContent = customization.HtmlContent;
  }
}
