using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Items.Models;

public record SearchItemsPayload : SearchPayload
{
  public new List<ItemSortOption> Sort { get; set; } = [];
}
