using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Parties;

public record PartySortOption : SortOption
{
  public new PartySort Field
  {
    get => Enum.Parse<PartySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public PartySortOption() : this(PartySort.Name)
  {
  }

  public PartySortOption(PartySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
