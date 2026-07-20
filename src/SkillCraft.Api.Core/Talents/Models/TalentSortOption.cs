using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Talents.Models;

public record TalentSortOption : SortOption
{
  public new TalentSort Field
  {
    get => Enum.Parse<TalentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public TalentSortOption(TalentSort field = TalentSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
