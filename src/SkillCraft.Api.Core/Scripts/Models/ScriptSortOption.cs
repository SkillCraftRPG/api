using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Scripts.Models;

public record ScriptSortOption : SortOption
{
  public new ScriptSort Field
  {
    get => Enum.Parse<ScriptSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ScriptSortOption(ScriptSort field = ScriptSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
