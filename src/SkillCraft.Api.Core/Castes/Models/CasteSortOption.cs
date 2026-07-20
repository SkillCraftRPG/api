using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Castes.Models;

public record CasteSortOption : SortOption
{
  public new CasteSort Field
  {
    get => Enum.Parse<CasteSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CasteSortOption(CasteSort field = CasteSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
