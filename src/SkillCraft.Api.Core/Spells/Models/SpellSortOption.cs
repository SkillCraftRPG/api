using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Spells.Models;

public record SpellSortOption : SortOption
{
  public new SpellSort Field
  {
    get => Enum.Parse<SpellSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public SpellSortOption(SpellSort field = SpellSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
