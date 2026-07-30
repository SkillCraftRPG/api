namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageAgeModel
{
  public int Teenager { get; set; }
  public int Adult { get; set; }
  public int Mature { get; set; }
  public int Venerable { get; set; }
}
