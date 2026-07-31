namespace SkillCraft.Api.Core.Items.Events;

public class ItemUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? Content { get; set; }

  public Change<double?>? Price { get; set; }
  public Change<double?>? Weight { get; set; }

  public ItemUpdated() : base()
  {
  }

  public ItemUpdated(Item item) : base(item)
  {
  }
}
