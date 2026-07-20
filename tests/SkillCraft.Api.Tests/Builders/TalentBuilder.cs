using Bogus;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ITalentBuilder
{
  ITalentBuilder WithId(Guid id);
  ITalentBuilder WithWorld(World? world);
  ITalentBuilder WithTier(int tier);
  ITalentBuilder WithName(string name);
  ITalentBuilder WithSummary(string? summary);
  ITalentBuilder WithHtmlContent(string? htmlContent);
  ITalentBuilder AllowMultiplePurchases(bool allowMultiplePurchases = true);
  ITalentBuilder WithSkill(Skill? skill);
  ITalentBuilder WithRequiredTalent(Talent? requiredTalent);

  Talent Build();
}

public class TalentBuilder : ITalentBuilder
{
  private readonly Faker _faker;

  private bool _allowMultiplePurchases = false;
  private string? _htmlContent = null;
  private Guid? _id = null;
  private string _name = "Talent";
  private Talent? _requiredTalent = null;
  private Skill? _skill = null;
  private string? _summary = null;
  private int _tier = 0;
  private World? _world = null;

  public TalentBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ITalentBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public ITalentBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ITalentBuilder WithTier(int tier)
  {
    _tier = tier;
    return this;
  }

  public ITalentBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ITalentBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ITalentBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
    return this;
  }

  public ITalentBuilder AllowMultiplePurchases(bool allowMultiplePurchases = true)
  {
    _allowMultiplePurchases = allowMultiplePurchases;
    return this;
  }

  public ITalentBuilder WithSkill(Skill? skill)
  {
    _skill = skill;
    return this;
  }

  public ITalentBuilder WithRequiredTalent(Talent? requiredTalent)
  {
    _requiredTalent = requiredTalent;
    return this;
  }

  public Talent Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Talent(world, _tier, _name, _id, _summary, _htmlContent, _allowMultiplePurchases, _skill, _requiredTalent);
  }

  public static Talent Competence(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Compétence")
    .WithSummary("Accorde un bonus permanent (+4) à l’Apprentissage.")
    .WithHtmlContent("Confère au personnage un bonus permanent (+4) à l’[Apprentissage](/regles/statistiques/apprentissage).")
    .AllowMultiplePurchases()
    .Build();
  public static Talent Melee(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Mêlée")
    .WithSummary("Forme au combat rapproché et au maniement des armes simples.")
    .WithHtmlContent("[Forme](/regles/equipement/armes/formation) le personnage au maniement des [armes simples](/regles/equipement/armes/simples) de mêlée.\n\nIl est également [formé](/regles/equipement/armures/formation) au port des [armures légères](/regles/equipement/armures/legeres) et à l’utilisation des [boucliers légers](/regles/equipement/boucliers).")
    .WithSkill(Skill.Melee)
    .Build();
  public static Talent FormationMartiale(Faker? faker = null, World? world = null, Talent? requiredTalent = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Formation martiale")
    .WithSummary("Accorde la maîtrise des armes et armures moyennes en combat.")
    .WithHtmlContent("Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).")
    .WithSkill(Skill.Melee)
    .WithRequiredTalent(requiredTalent ?? Melee(faker, world))
    .Build();
  public static Talent Charge(Faker? faker = null, World? world = null, Talent? requiredTalent = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithTier(1)
    .WithName("Charge")
    .WithSummary("Permet d’attaquer en courant et de renverser la cible touchée.")
    .WithHtmlContent("Le personnage peut effectuer une [attaque de mêlée](/regles/combat/attaque/melee) en [action libre](/regles/combat/deroulement/tour) lorsqu’il [court](/regles/aventure/mouvement/types) en ligne droite sur une distance d’au moins 4,5 mètres immédiatement avant d’effectuer cette [attaque](/regles/combat/attaque).\n\nIl peut ajouter un bonus (+5) à son [test](/regles/competences/tests) d’attaque ou un [dé de dégâts](/regles/combat/degats/jet) supplémentaire.\n\nSi la [taille](/regles/especes/taille) de la cible est inférieure ou égale à celle du personnage, la cible doit effectuer un [jet de sauvegarde](/regles/competences/tests/sauvegarde) d’[Acrobaties](/regles/competences/acrobaties) ou d’[Athlétisme](/regles/competences/athletisme).\n\nLa [difficulté](/regles/competences/tests/difficulte) correspond au résultat du test de l’attaque effectuée par le personnage.\n\nEn cas d’échec, elle est repoussée de 3 mètres, ou elle est repoussée de 1,5 mètres et tombe [renversée](/regles/combat/conditions/renverse) au sol, au choix du personnage.")
    .WithRequiredTalent(requiredTalent ?? Melee(faker, world))
    .Build();
}
