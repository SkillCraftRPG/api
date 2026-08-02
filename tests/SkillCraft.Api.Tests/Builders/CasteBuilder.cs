using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ICasteBuilder
{
  ICasteBuilder WithId(CasteId casteId);
  ICasteBuilder WithWorld(World? world);
  ICasteBuilder WithName(string name);
  ICasteBuilder WithSummary(string? summary);
  ICasteBuilder WithContent(string? content);
  ICasteBuilder WithSkill(Skill? skill);
  ICasteBuilder WithWealthRoll(string? wealthRoll);
  ICasteBuilder WithFeature(Feature? feature);

  Caste Build();
}

public class CasteBuilder : ICasteBuilder
{
  private readonly Faker _faker;

  private CasteId? _casteId = null;
  private string? _content = null;
  private Feature? _feature = null;
  private string _name = "Caste";
  private Skill? _skill = null;
  private string? _summary = null;
  private string? _wealthRoll = null;
  private World? _world = null;

  public CasteBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ICasteBuilder WithId(CasteId casteId)
  {
    _casteId = casteId;
    return this;
  }

  public ICasteBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ICasteBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ICasteBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ICasteBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public ICasteBuilder WithSkill(Skill? skill)
  {
    _skill = skill;
    return this;
  }

  public ICasteBuilder WithWealthRoll(string? wealthRoll)
  {
    _wealthRoll = wealthRoll;
    return this;
  }

  public ICasteBuilder WithFeature(Feature? feature)
  {
    _feature = feature;
    return this;
  }

  public Caste Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    ActorId actorId = world.OwnerId.ActorId;
    Name name = new(_name);

    Caste caste = _casteId.HasValue
      ? new(_casteId.Value, name, actorId)
      : new(world, name, actorId);

    caste.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);
    caste.SetRules(_skill, Roll.TryCreate(_wealthRoll), _feature, actorId);

    return caste;
  }

  public static Caste Artisan(Faker? faker = null, World? world = null) => new CasteBuilder(faker)
    .WithWorld(world)
    .WithName("Artisan")
    .WithSummary("Expert des métiers manuels, membre d’une organisation d’artisans.")
    .WithContent("L’artisan est un expert d’un procédé de transformation des matières brutes.\n\nIl peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.")
    .WithSkill(Skill.Crafting)
    .WithWealthRoll("8d6")
    .WithFeature(new Feature(new Name("Professionnel"), Content.TryCreate("Grâce à ses apprentissages et à ses réalisations, le personnage est membre d’une organisation de professionnels comme lui, ou il connait ces organisations.\n\nS’il ne peut subvenir à ses besoins, il n’aura aucun mal à trouver du travail grâce à ces organisations afin de couvrir minimalement ces [dépenses](/regles/equipement/depenses).\n\nCes organisations possèdent souvent un pouvoir politique important, ce qui peut l’aider à rencontrer des gens importants, à rallier des fidèles à une cause ou à mettre la main sur des matériaux rares.\n\nIl connait également la base du fonctionnement des systèmes économiques auxquels il a participé.")))
    .Build();
}
