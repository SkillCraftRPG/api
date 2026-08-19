using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ITalentBuilder
{
  ITalentBuilder WithId(TalentId talentId);
  ITalentBuilder WithWorld(World? world);
  ITalentBuilder WithTier(int tier);
  ITalentBuilder WithName(string name);
  ITalentBuilder WithSummary(string? summary);
  ITalentBuilder WithContent(string? content);
  ITalentBuilder AllowMultiplePurchases(bool allowMultiplePurchases = true);
  ITalentBuilder WithSkill(Skill? skill);
  ITalentBuilder WithRequiredTalent(Talent? requiredTalent);

  Talent Build();
}

public class TalentBuilder : ITalentBuilder
{
  private readonly Faker _faker;

  private bool _allowMultiplePurchases = false;
  private string? _content = null;
  private string _name = "Talent";
  private Talent? _requiredTalent = null;
  private Skill? _skill = null;
  private string? _summary = null;
  private TalentId? _talentId = null;
  private int _tier = 0;
  private World? _world = null;

  public TalentBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ITalentBuilder WithId(TalentId talentId)
  {
    _talentId = talentId;
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

  public ITalentBuilder WithContent(string? content)
  {
    _content = content;
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
    ActorId actorId = world.OwnerId.ActorId;
    TalentTier tier = new(_tier);
    Name name = new(_name);

    Talent talent = _talentId.HasValue
      ? new(_talentId.Value, tier, name, actorId)
      : new(world, tier, name, actorId);

    talent.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);
    talent.SetRules(_allowMultiplePurchases, _skill, actorId);
    talent.SetRequirements(_requiredTalent, actorId);

    return talent;
  }

  public static Talent Artisanat(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Artisanat")
    .WithSummary("Forme à l’usage d’outils d’artisan et à la réparation d’objets.")
    .WithContent("Le personnage est formé à l’utilisation d’un nombre d’[outils](/regles/equipement/outils) ou de [trousses d’artisan](/regles/equipement/outils) égal à son [rang](/regles/competences/rang) d’[Artisanat](/regles/competences/artisanat).\n\nS’il est formé à l’utilisation de la [trousse de réparation](/regles/equipement/outils), il peut tenter de [réparer](/regles/equipement/reparation) un objet. Par une [action](/regles/combat/deroulement/tour), il peut utiliser une des dix charges de la trousse et effectue un [test](/regles/competences/tests) d’Artisanat.\n\n- Si l’objet est une [armure](/regles/equipement/armures) ou un [bouclier](/regles/equipement/boucliers), le nombre de points de [Résistance](/regles/equipement/resistance) restaurés est égal au résultat du test divisé par 5 (minimum 1).\n- Si l’objet est une [arme](/regles/equipement/armes), le [degré de difficulté](/regles/competences/tests/difficulte) est égal au bonus à l’[Attaque](/regles/equipement/armes/attaque) de l’arme multiplié par 5. L’arme est réparée en cas de réussite.\n- Sinon, la difficulté est moyenne. L’objet est réparé en cas de réussite.")
    .WithSkill(Skill.Crafting)
    .Build();

  public static Talent Charge(Faker? faker = null, World? world = null, Talent? requiredTalent = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithTier(1)
    .WithName("Charge")
    .WithSummary("Permet d’attaquer en courant et de renverser la cible touchée.")
    .WithContent("Le personnage peut effectuer une [attaque de mêlée](/regles/combat/attaque/melee) en [action libre](/regles/combat/deroulement/tour) lorsqu’il [court](/regles/aventure/mouvement/types) en ligne droite sur une distance d’au moins 4,5 mètres immédiatement avant d’effectuer cette [attaque](/regles/combat/attaque).\n\nIl peut ajouter un bonus (+5) à son [test](/regles/competences/tests) d’attaque ou un [dé de dégâts](/regles/combat/degats/jet) supplémentaire.\n\nSi la [taille](/regles/especes/taille) de la cible est inférieure ou égale à celle du personnage, la cible doit effectuer un [jet de sauvegarde](/regles/competences/tests/sauvegarde) d’[Acrobaties](/regles/competences/acrobaties) ou d’[Athlétisme](/regles/competences/athletisme).\n\nLa [difficulté](/regles/competences/tests/difficulte) correspond au résultat du test de l’attaque effectuée par le personnage.\n\nEn cas d’échec, elle est repoussée de 3 mètres, ou elle est repoussée de 1,5 mètres et tombe [renversée](/regles/combat/conditions/renverse) au sol, au choix du personnage.")
    .WithRequiredTalent(requiredTalent ?? Melee(faker, world))
    .Build();

  public static Talent Competence(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Compétence")
    .WithSummary("Accorde un bonus permanent (+4) à l’Apprentissage.")
    .WithContent("Confère au personnage un bonus permanent (+4) à l’[Apprentissage](/regles/statistiques/apprentissage).")
    .AllowMultiplePurchases()
    .Build();

  public static Talent Connaissance(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Connaissance")
    .WithSummary("Accorde des domaines de savoir égaux au rang de Connaissance.")
    .WithContent("Le personnage acquiert des connaissances sur un nombre de domaines spécifiques égal à son [rang](/regles/competences/rang) de [Connaissance](/regles/competences/connaissance).\n\nLe joueur et le maître de jeu collaborent afin de définir les sujets pouvant constituer un domaine de connaissances.")
    .WithSkill(Skill.Knowledge)
    .Build();

  public static Talent FormationMartiale(Faker? faker = null, World? world = null, Talent? requiredTalent = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Formation martiale")
    .WithSummary("Accorde la maîtrise des armes et armures moyennes en combat.")
    .WithContent("Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).")
    .WithSkill(Skill.Melee)
    .WithRequiredTalent(requiredTalent ?? Melee(faker, world))
    .Build();

  public static Talent LangueSupplementaire(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Langue supplémentaire")
    .WithSummary("Le personnage apprend une langue supplémentaire de son choix.")
    .WithContent("Il doit avoir accès à un interlocuteur pouvant lui apprendre cette langue, ou suffisamment de sources écrites.\n\nLa langue doit également figurer parmi les catégories de langues auxquelles il a accès.")
    .AllowMultiplePurchases()
    .Build();

  public static Talent Furtivite(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Furtivité")
    .WithSummary("Permet de se cacher même lorsqu’on est vu, si légèrement obscurci.")
    .WithContent("Permet au personnage de tenter de [se cacher](/regles/combat/activites/cacher) d’une créature qui le voit lorsqu’il est [légèrement obscurci](/regles/aventure/environnement/vision).")
    .WithSkill(Skill.Stealth)
    .Build();

  public static Talent Melee(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Mêlée")
    .WithSummary("Forme au combat rapproché et au maniement des armes simples.")
    .WithContent("[Forme](/regles/equipement/armes/formation) le personnage au maniement des [armes simples](/regles/equipement/armes/simples) de mêlée.\n\nIl est également [formé](/regles/equipement/armures/formation) au port des [armures légères](/regles/equipement/armures/legeres) et à l’utilisation des [boucliers légers](/regles/equipement/boucliers).")
    .WithSkill(Skill.Melee)
    .Build();

  public static Talent Orientation(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Orientation")
    .WithSummary("Forme au tir à distance et au port des armures légères.")
    .WithContent("[Forme](/regles/equipement/armes/formation) le personnage au maniement des [armes simples](/regles/equipement/armes/simples) à distance.\n\nIl est également [formé au port](/regles/equipement/armures/formation) des [armures légères](/regles/equipement/armures/legeres) et à l’utilisation des [boucliers légers](/regles/equipement/boucliers).")
    .WithSkill(Skill.Orientation)
    .Build();

  public static Talent Perception(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Perception")
    .WithSummary("Affine les sens pour lire et entendre à distance.")
    .WithContent("Le personnage peut déceler des détails ou du texte précis depuis un document, un objet ou toute autre surface.\n\nIl peut également entendre clairement une discussion chuchotée.\n\nLa portée de cette capacité est de 30 centimètres multipliés par son [test passif](/regles/competences/tests/passif) de [Perception](/regles/competences/perception).")
    .WithSkill(Skill.Perception)
    .Build();

  public static Talent Roublardise(Faker? faker = null, World? world = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Roublardise")
    .WithSummary("Forme aux outils de voleur et à la manipulation discrète d’objets.")
    .WithContent("Le personnage est formé à l’utilisation des [outils de voleur](/regles/equipement/outils).\n\nÉgalement, il peut effectuer l’activité [Objet](/regles/combat/activites/objet) (une [action](/regles/combat/deroulement/tour)) afin de manipuler (déplacer, ou dissimuler sur lui ou quelqu’un d’autre) un objet d’au plus la grosseur de la paume de sa main.")
    .WithSkill(Skill.Thievery)
    .Build();

  public static Talent Trousses(Faker? faker = null, World? world = null, Talent? requiredTalent = null) => new TalentBuilder(faker)
    .WithWorld(world)
    .WithName("Trousses")
    .WithSummary("Forme aux trousses de déguisement et de falsification via Roublardise.")
    .WithContent("Le personnage est formé à l’utilisation de la [trousse de déguisement](/regles/equipement/outils) ([Tromperie](/regles/competences/tromperie)) et de la [trousse de falsification](/regles/equipement/outils) ([Linguistique](/regles/competences/linguistique)).\n\nLorsqu’il utilise une de ces trousses, il peut le faire par un [test](/regles/competences/tests) de [Roublardise](/regles/competences/roublardise) plutôt que par un test de Tromperie ou de Linguistique.")
    .WithRequiredTalent(requiredTalent ?? Roublardise(faker, world))
    .Build();
}
