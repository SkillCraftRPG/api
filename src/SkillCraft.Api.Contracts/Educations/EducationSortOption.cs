using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Educations;

public record EducationSortOption : SortOption
{
  public new EducationSort Field
  {
    get => Enum.Parse<EducationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public EducationSortOption() : this(EducationSort.Name)
  {
  }

  public EducationSortOption(EducationSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
