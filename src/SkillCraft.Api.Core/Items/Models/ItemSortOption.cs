using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Items.Models;

public record ItemSortOption : SortOption
{
  public new ItemSort Field
  {
    get => Enum.Parse<ItemSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ItemSortOption(ItemSort field = ItemSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
