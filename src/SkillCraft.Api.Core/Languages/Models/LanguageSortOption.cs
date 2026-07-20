using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Languages.Models;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption(LanguageSort field = LanguageSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
