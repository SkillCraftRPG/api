using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Customizations;

public record CustomizationSortOption : SortOption
{
  public new CustomizationSort Field
  {
    get => Enum.Parse<CustomizationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CustomizationSortOption() : this(CustomizationSort.Name)
  {
  }

  public CustomizationSortOption(CustomizationSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
