namespace SkillCraft.Api.Core.Items.Events;

public class ItemCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public double? Price { get; set; }
  public double? Weight { get; set; }

  public ItemCreated() : base()
  {
  }

  public ItemCreated(Item item) : base(item)
  {
    Name = item.Name;
    Summary = item.Summary;
    Content = item.Content;

    Price = item.Price;
    Weight = item.Weight;
  }
}
