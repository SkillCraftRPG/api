using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Specializations;

public record SpecializationSortOption : SortOption
{
  public new SpecializationSort Field
  {
    get => Enum.Parse<SpecializationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public SpecializationSortOption() : this(SpecializationSort.Name)
  {
  }

  public SpecializationSortOption(SpecializationSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
