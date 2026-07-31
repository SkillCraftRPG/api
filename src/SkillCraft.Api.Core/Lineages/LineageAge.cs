using FluentValidation;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageAge
{
  int? Teenager { get; }
  int? Adult { get; }
  int? Mature { get; }
  int? Venerable { get; }
}

public record LineageAge : ILineageAge
{
  public int? Teenager { get; }
  public int? Adult { get; }
  public int? Mature { get; }
  public int? Venerable { get; }

  public LineageAge()
  {
  }

  [JsonConstructor]
  public LineageAge(int? teenager, int? adult, int? mature, int? venerable)
  {
    Teenager = teenager;
    Adult = adult;
    Mature = mature;
    Venerable = venerable;
  }

  public LineageAge(Lineage lineage) : this(lineage.Teenager, lineage.Adult, lineage.Mature, lineage.Venerable)
  {
  }
}

internal class LineageAgeValidator : AbstractValidator<ILineageAge>
{
  public LineageAgeValidator()
  {
    When(x => x.Teenager is null || x.Adult is null || x.Mature is null || x.Venerable is null, () =>
    {
      RuleFor(x => x.Teenager).Null();
      RuleFor(x => x.Adult).Null();
      RuleFor(x => x.Mature).Null();
      RuleFor(x => x.Venerable).Null();
    }).Otherwise(() =>
    {
      RuleFor(x => x.Teenager).NotNull().GreaterThan(0);
      RuleFor(x => x.Adult).NotNull().GreaterThan(age => age.Teenager);
      RuleFor(x => x.Mature).NotNull().GreaterThan(age => age.Adult);
      RuleFor(x => x.Venerable).NotNull().GreaterThan(age => age.Mature);
    });
  }
}
