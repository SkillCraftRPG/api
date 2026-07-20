using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Customizations.Models;

public record CustomizationSortOption : SortOption
{
  public new CustomizationSort Field
  {
    get => Enum.Parse<CustomizationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CustomizationSortOption(CustomizationSort field = CustomizationSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
