namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageAgeModel : ILineageAge
{
  public int? Teenager { get; set; }
  public int? Adult { get; set; }
  public int? Mature { get; set; }
  public int? Venerable { get; set; }

  public LineageAgeModel()
  {
  }

  public LineageAgeModel(int? teenager, int? adult, int? mature, int? venerable)
  {
    Teenager = teenager;
    Adult = adult;
    Mature = mature;
    Venerable = venerable;
  }

  public LineageAgeModel(ILineageAge age) : this(age.Teenager, age.Adult, age.Mature, age.Venerable)
  {
  }
}
