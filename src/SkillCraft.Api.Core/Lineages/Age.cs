using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Lineages.Validators;

namespace SkillCraft.Api.Core.Lineages;

public record Age : IAge
{
  public int Teenager { get; }
  public int Adult { get; }
  public int Mature { get; }
  public int Venerable { get; }

  public Age()
  {
  }

  public Age(IAge age) : this(age.Teenager, age.Adult, age.Mature, age.Venerable)
  {
  }

  [JsonConstructor]
  public Age(int teenager, int adult, int mature, int venerable)
  {
    Teenager = teenager;
    Adult = adult;
    Mature = mature;
    Venerable = venerable;
    new AgeValidator().ValidateAndThrow(this);
  }
}
