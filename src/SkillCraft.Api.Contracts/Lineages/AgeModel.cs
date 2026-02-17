namespace SkillCraft.Api.Contracts.Lineages;

public record AgeModel : IAge
{
  public int? Teenager { get; set; }
  public int? Adult { get; set; }
  public int? Mature { get; set; }
  public int? Venerable { get; set; }

  public AgeModel(int? teenager = null, int? adult = null, int? mature = null, int? venerable = null)
  {
    Teenager = teenager;
    Adult = adult;
    Mature = mature;
    Venerable = venerable;
  }
}
