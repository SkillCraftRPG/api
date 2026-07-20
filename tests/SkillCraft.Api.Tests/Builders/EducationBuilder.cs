using Bogus;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IEducationBuilder
{
  IEducationBuilder WithId(Guid id);
  IEducationBuilder WithWorld(World? world);
  IEducationBuilder WithName(string name);
  IEducationBuilder WithSummary(string? summary);
  IEducationBuilder WithHtmlContent(string? htmlContent);
  IEducationBuilder WithSkill(Skill? skill);
  IEducationBuilder WithWealthMultiplier(int? wealthMultiplier);
  IEducationBuilder WithFeature(Feature? feature);

  Education Build();
}

public class EducationBuilder : IEducationBuilder
{
  private readonly Faker _faker;

  private Feature? _feature = null;
  private string? _htmlContent = null;
  private Guid? _id = null;
  private string _name = "Education";
  private Skill? _skill = null;
  private string? _summary = null;
  private int? _wealthMultiplier = null;
  private World? _world = null;

  public EducationBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IEducationBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public IEducationBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public IEducationBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public IEducationBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public IEducationBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
    return this;
  }

  public IEducationBuilder WithSkill(Skill? skill)
  {
    _skill = skill;
    return this;
  }

  public IEducationBuilder WithWealthMultiplier(int? wealthMultiplier)
  {
    _wealthMultiplier = wealthMultiplier;
    return this;
  }

  public IEducationBuilder WithFeature(Feature? feature)
  {
    _feature = feature;
    return this;
  }

  public Education Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Education(world, _name, _id, _summary, _htmlContent, _skill, _wealthMultiplier, _feature);
  }

  public static Education Judicieux(Faker? faker = null, World? world = null) => new EducationBuilder(faker)
    .WithWorld(world)
    .WithName("Judicieux")
    .WithSummary("Esprit posé et analytique, prêt à guider par des décisions avisées.")
    .WithHtmlContent("Peu importe le mode de vie dans lequel il a été élevé, le personnage prend des décisions sensées et éclairées au moment opportun.\n\nIl saisit les opportunités et on lui demande souvent conseil.\n\nIl sait mettre en exécution des plans complexes et trier les informations pertinentes.")
    .WithSkill(Skill.Orientation)
    .WithWealthMultiplier(10)
    .WithFeature(new Feature("Conseiller avisé", "La nature calme et analytique du personnage lui permet d’être reconnu rapidement pour son jugement sûr.\n\nIl peut ajouter un bonus égal à son [tiers](/regles/personnages/progression/tiers) (minimum 1) à ses [tests](/regles/competences/tests) d’[Intuition](/regles/competences/intuition) ou d’[Investigation](/regles/competences/investigation) effectués afin de comprendre un plan, évaluer un risque ou choisir la meilleure approche de manière objective.\n\nÉgalement, il ajoute également ce bonus à ses tests de [Diplomatie](/regles/competences/diplomatie) effectués afin de convaincre un individue rationnel."))
    .Build();
}
