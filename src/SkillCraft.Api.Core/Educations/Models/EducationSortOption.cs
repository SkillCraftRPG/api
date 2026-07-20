using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Educations.Models;

public record EducationSortOption : SortOption
{
  public new EducationSort Field
  {
    get => Enum.Parse<EducationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public EducationSortOption(EducationSort field = EducationSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
