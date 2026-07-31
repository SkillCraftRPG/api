using SkillCraft.Api.Core.Items.Events;

namespace SkillCraft.Api.Core.Items;

public record ItemSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public double? Price { get; }
  public double? Weight { get; }

  public ItemSnapshot(Item item)
  {
    Name = item.Name;
    Summary = item.Summary;
    Content = item.Content;

    Price = item.Price;
    Weight = item.Weight;
  }

  public ItemUpdated? Compare(Item item)
  {
    int changes = 0;
    ItemUpdated record = new(item);

    if (Name != item.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, item.Name);
    }

    if (Summary != item.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, item.Summary);
    }

    if (Content != item.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, item.Content);
    }

    if (Price != item.Price)
    {
      changes++;
      record.Price = new Change<double?>(Price, item.Price);
    }

    if (Weight != item.Weight)
    {
      changes++;
      record.Weight = new Change<double?>(Weight, item.Weight);
    }

    return changes < 1 ? null : record;
  }
}
