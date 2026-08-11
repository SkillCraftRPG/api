using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Characters.Models;

public record SearchCharactersPayload : SearchPayload
{
  public Guid? LineageId { get; set; }
  public Guid? CasteId { get; set; }
  public Guid? EducationId { get; set; }

  public new List<CharacterSortOption> Sort { get; set; } = [];
}
