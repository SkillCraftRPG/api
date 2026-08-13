namespace SkillCraft.Api.Core.Items.Models;

public record AttunementModel : IAttunement
{
  public bool IsRequired { get; set; }
  public string? Requirements { get; set; }
}
