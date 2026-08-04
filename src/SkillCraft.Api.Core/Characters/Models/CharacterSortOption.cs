using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterSortOption : SortOption
{
  public new CharacterSort Field
  {
    get => Enum.Parse<CharacterSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CharacterSortOption(CharacterSort field = CharacterSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
