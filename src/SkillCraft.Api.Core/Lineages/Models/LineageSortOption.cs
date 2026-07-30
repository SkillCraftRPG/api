using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageSortOption : SortOption
{
  public new LineageSort Field
  {
    get => Enum.Parse<LineageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LineageSortOption(LineageSort field = LineageSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
