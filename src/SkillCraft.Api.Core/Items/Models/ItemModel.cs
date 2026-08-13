using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Items.Models;

public class ItemModel : Aggregate
{
  public ItemCategory Category { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public int? Price { get; set; }
  public int? Weight { get; set; }

  public ItemRarity? Rarity { get; set; }
  public ItemChargesModel? Charges { get; set; }
  public MagicItemModel? Magic { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
