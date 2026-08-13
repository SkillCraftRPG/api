using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Items.Models;

public record SearchItemsPayload : SearchPayload
{
  public ItemCategory? Category { get; set; }
  public ItemRarity? Rarity { get; set; }

  public new List<ItemSortOption> Sort { get; set; } = [];
}
