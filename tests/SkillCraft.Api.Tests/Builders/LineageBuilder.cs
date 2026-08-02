using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ILineageBuilder
{
  ILineageBuilder WithId(LineageId lineageId);
  ILineageBuilder WithWorld(World? world);
  ILineageBuilder WithParent(Lineage? parent);
  ILineageBuilder WithName(string name);
  ILineageBuilder WithSummary(string? summary);
  ILineageBuilder WithContent(string? content);
  ILineageBuilder WithFeatures(IEnumerable<Feature>? features);
  ILineageBuilder WithLanguages(IEnumerable<LanguageId>? languageIds = null, int extra = 0, string? content = null);
  ILineageBuilder WithNames(
    IEnumerable<string>? family = null,
    IEnumerable<string>? female = null,
    IEnumerable<string>? male = null,
    IEnumerable<string>? unisex = null,
    IEnumerable<NameCategory>? custom = null,
    string? content = null);
  ILineageBuilder WithSpeeds(int? walk = null, int? climb = null, int? swim = null, int? fly = null, bool hover = false, int? burrow = null);
  ILineageBuilder WithSize(SizeCategory category, string? height = null);
  ILineageBuilder WithWeight(string? malnutrition = null, string? skinny = null, string? normal = null, string? overweight = null, string? obese = null);
  ILineageBuilder WithAge(int? teenager = null, int? adult = null, int? mature = null, int? venerable = null);

  Lineage Build();
}

public class LineageBuilder : ILineageBuilder
{
  private readonly Faker _faker;

  private int? _adult = null;
  private int? _burrow = null;
  private int? _climb = null;
  private string? _content = null;
  private IEnumerable<NameCategory> _customNames = [];
  private IEnumerable<Feature> _features = [];
  private int _extraLanguages = 0;
  private int? _fly = null;
  private IEnumerable<string> _familyNames = [];
  private IEnumerable<string> _femaleNames = [];
  private string? _height = null;
  private bool _hover = false;
  private LineageId? _lineageId = null;
  private string? _languagesContent = null;
  private IEnumerable<LanguageId> _languageIds = [];
  private IEnumerable<string> _maleNames = [];
  private string? _malnutrition = null;
  private int? _mature = null;
  private string _name = "Lineage";
  private string? _namesContent = null;
  private string? _normalWeight = null;
  private string? _obese = null;
  private string? _overweight = null;
  private Lineage? _parent = null;
  private SizeCategory _sizeCategory = SizeCategory.Medium;
  private string? _skinny = null;
  private string? _summary = null;
  private int? _swim = null;
  private int? _teenager = null;
  private IEnumerable<string> _unisexNames = [];
  private int? _venerable = null;
  private int? _walk = null;
  private World? _world = null;

  public LineageBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ILineageBuilder WithId(LineageId lineageId)
  {
    _lineageId = lineageId;
    return this;
  }

  public ILineageBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ILineageBuilder WithParent(Lineage? parent)
  {
    _parent = parent;
    return this;
  }

  public ILineageBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ILineageBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ILineageBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public ILineageBuilder WithFeatures(IEnumerable<Feature>? features)
  {
    _features = features ?? [];
    return this;
  }

  public ILineageBuilder WithLanguages(IEnumerable<LanguageId>? languageIds = null, int extra = 0, string? content = null)
  {
    _languageIds = languageIds ?? [];
    _extraLanguages = extra;
    _languagesContent = content;
    return this;
  }

  public ILineageBuilder WithNames(
    IEnumerable<string>? family = null,
    IEnumerable<string>? female = null,
    IEnumerable<string>? male = null,
    IEnumerable<string>? unisex = null,
    IEnumerable<NameCategory>? custom = null,
    string? content = null)
  {
    _familyNames = family ?? [];
    _femaleNames = female ?? [];
    _maleNames = male ?? [];
    _unisexNames = unisex ?? [];
    _customNames = custom ?? [];
    _namesContent = content;
    return this;
  }

  public ILineageBuilder WithSpeeds(int? walk = null, int? climb = null, int? swim = null, int? fly = null, bool hover = false, int? burrow = null)
  {
    _walk = walk;
    _climb = climb;
    _swim = swim;
    _fly = fly;
    _hover = hover;
    _burrow = burrow;
    return this;
  }

  public ILineageBuilder WithSize(SizeCategory category, string? height = null)
  {
    _sizeCategory = category;
    _height = height;
    return this;
  }

  public ILineageBuilder WithWeight(string? malnutrition = null, string? skinny = null, string? normal = null, string? overweight = null, string? obese = null)
  {
    _malnutrition = malnutrition;
    _skinny = skinny;
    _normalWeight = normal;
    _overweight = overweight;
    _obese = obese;
    return this;
  }

  public ILineageBuilder WithAge(int? teenager = null, int? adult = null, int? mature = null, int? venerable = null)
  {
    _teenager = teenager;
    _adult = adult;
    _mature = mature;
    _venerable = venerable;
    return this;
  }

  public Lineage Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    ActorId actorId = world.OwnerId.ActorId;
    Name name = new(_name);

    Lineage lineage = _lineageId.HasValue
      ? new(_lineageId.Value, name, _parent, actorId)
      : new(world, name, _parent, actorId);

    lineage.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);
    lineage.SetFeatures(_features, actorId);

    Dictionary<string, IReadOnlyCollection<string>> custom = new();
    foreach (NameCategory category in _customNames)
    {
      custom[category.Category] = category.Values;
    }

    lineage.SetLanguages(new LineageLanguages([.. _languageIds], _extraLanguages, Content.TryCreate(_languagesContent)), actorId);
    lineage.SetNames(new LineageNames([.. _familyNames], [.. _femaleNames], [.. _maleNames], [.. _unisexNames], custom, Content.TryCreate(_namesContent)), actorId);
    lineage.SetSpeeds(new LineageSpeeds(_walk, _climb, _swim, _fly, _hover, _burrow), actorId);
    lineage.SetTraits(
      new LineageSize(_sizeCategory, Roll.TryCreate(_height)),
      new LineageWeight(Roll.TryCreate(_malnutrition), Roll.TryCreate(_skinny), Roll.TryCreate(_normalWeight), Roll.TryCreate(_overweight), Roll.TryCreate(_obese)),
      new LineageAge(_teenager, _adult, _mature, _venerable),
      actorId);

    return lineage;
  }

  public static Lineage Humain(Faker? faker = null, World? world = null) => new LineageBuilder(faker)
    .WithWorld(world)
    .WithName("Humain")
    .WithSummary("Espèce adaptable et ambitieuse héritière d’un empire fragmenté.")
    .WithContent("Les humains représentent le commun des mortels. Répandus aux quatre coins du monde, ils s’adaptent avec aisance aux climats, aux cultures et aux bouleversements qui façonnent les civilisations. Héritiers d’un vaste empire ayant autrefois dominé la majeure partie de l’Ouespéro, ils vivent désormais au sein d’une mosaïque de royaumes, de cités libres et d’empires revendiquant un héritage souvent contesté. Leur culture mêle traditions impériales, foi organisée et anciennes coutumes guerrières, tandis que leur courte espérance de vie nourrit leur ambition et leur désir d’accomplissement. Perçus comme éphémères mais résilients, ils occupent une place centrale dans les alliances, les conflits et les échanges du monde connu.")
    .WithFeatures(HumainFeatures())
    .WithLanguages(extra: 2)
    .WithNames(content: "Les humains portent généralement un prénom et un nom de famille.")
    .WithSpeeds(walk: 6)
    .WithSize(SizeCategory.Medium, "140+2d20")
    .WithWeight("10+1d4", "14+1d4", "18+1d6", "24+1d6", "30+1d10")
    .WithAge(8, 15, 30, 55)
    .Build();

  public static Lineage Elfe(Faker? faker = null, World? world = null) => new LineageBuilder(faker)
    .WithWorld(world)
    .WithName("Elfe")
    .WithSummary("Peuple ancien et longévif héritier d’une civilisation engloutie.")
    .WithContent("Les Elfes sont des êtres longévifs originaires d’Erdimar, un ancien continent aujourd’hui associé aux ruines d’une civilisation engloutie par un déluge oublié. Arrivés en Ouespéro après avoir traversé Hyperborée et les Gelvas il y a plusieurs millénaires, ils y fondèrent de vastes royaumes dont les vestiges parsèment encore l’Ouest et le Centre du continent, particulièrement dans l’archipel des Triskîles. Connus pour leur silhouette élancée, leurs sens aiguisés et leur maîtrise des arts occultes, ils entretiennent un profond attachement aux traditions, à la mémoire et à l’ordre naturel. Leur lente démographie les pousse à préserver des bastions anciens plutôt qu’à étendre rapidement leurs territoires, ce qui nourrit leur rivalité historique avec les [Nains](/regles/especes/nain) et leur haine farouche des [Orques](/regles/especes/orque), qu’ils considèrent comme des Elfes corrompus.")
    .WithFeatures(ElfeFeatures())
    .WithLanguages(extra: 1)
    .WithNames(content: "Les Elfes portent généralement un prénom et un nom de famille.")
    .WithSpeeds(walk: 6)
    .WithSize(SizeCategory.Medium, "130+3d20")
    .WithWeight("9+1d3", "12+1d4", "16+1d6", "22+1d6", "28+1d8")
    .WithAge(30, 100, 275, 750)
    .Build();

  public static Lineage HautElfe(Faker? faker = null, World? world = null, Lineage? parent = null, Language? celfique = null) => new LineageBuilder(faker)
    .WithWorld(world)
    .WithParent(parent ?? Elfe(faker, world))
    .WithName("Haut-Elfe")
    .WithSummary("Héritiers stellaires de royaumes elfiques raffinés et érudits.")
    .WithContent("Les Hauts-Elfes privilégient l’ordre, l’érudition et la stabilité, qu’ils considèrent comme les fondations de toute civilisation durable. Héritiers d’un antique royaume forestier fondé dans l’Ouest de la Sarénie il y a plus de deux millénaires, ils ont développé une culture raffinée où astrologie, magie et savoir occupent une place centrale. Leur expansion vers les Triskîles mena à la fondation de royaumes sur Alnar et Ellesdales, bien que cette dernière région se soit fragmentée au fil des siècles en principautés rivales. Diplomates et marchands influents, les Hauts-Elfes entretiennent généralement de bonnes relations avec les peuples civilisés, mais les ambitions territoriales des Dallois menacent désormais l’équilibre fragile d’Ellesdales. Nés sous des constellations considérées sacrées, ils portent souvent sur eux le symbole de l’étoile ayant marqué leur destinée.")
    .WithLanguages(celfique is null ? null : [celfique.Id], extra: 1)
    .WithNames(
      family: ["Adlegor", "Charimon", "Galanodel", "Kaendere", "Liadon", "Morwen", "Nodir", "Raelden", "Sonomir", "Talath"],
      female: ["Althaea", "Ceanoise", "Elenna", "Leshanna", "Magilin", "Naeva", "Onoraid", "Sariel", "Tibenna", "Valanthe"],
      male: ["Aelar", "Berrian", "Erevan", "Feirion", "Hononn", "Ivellios", "Kaimear", "Peren", "Saemainn", "Therion"])
    .Build();

  public static Lineage Nain(Faker? faker = null, World? world = null) => new LineageBuilder(faker)
    .WithWorld(world)
    .WithName("Nain")
    .WithSummary("Peuple montagnard ancien, fier et résilient de la Tyrgie.")
    .WithContent("Les Nains sont un peuple ancien ayant régné durant cinq millénaires sur la Tyrgie, principalement autour des chaînes montagneuses de l’Échine. Trapus, robustes et adaptés à la vie souterraine, ils ont bâti d’immenses royaumes de pierre dont les vestiges marquent encore le continent. Leur culture valorise la forge, les serments, les lignées et le devoir envers la communauté, chaque individu étant appelé à maîtriser plusieurs savoir-faire afin de soutenir son clan. Les légendes divergent quant à leurs origines : certains les disent issus des Géants, d’autres façonnés par les Grands Dragons, tandis que plusieurs soutiennent qu’ils ne sont que le produit d’une longue adaptation aux montagnes. Malgré la chute de leurs derniers royaumes il y a près de mille ans, les Nains demeurent fiers, résilients et profondément méfiants envers leurs ennemis ancestraux.")
    .WithFeatures(NainFeatures())
    .WithLanguages(extra: 1)
    .WithNames(content: "Les Nains habitent en clans rassemblant les membres d’une même famille. Lorsqu’un Nain naît, les aînés du clan lui assignent un prénom et il adopte le nom de famille du clan. Ces noms appartiennent au clan, si bien que lorsqu’un individu déshonore son clan, ses noms peuvent lui être retirés. Il est ensuite interdit à l’individu de porter ces noms par les lois sacrées naines.")
    .WithSpeeds(walk: 5)
    .WithSize(SizeCategory.Medium, "120+3d10")
    .WithWeight("22+1d4", "26+1d6", "32+1d8", "40+1d8", "48+1d10")
    .WithAge(15, 50, 150, 350)
    .Build();

  public static IReadOnlyCollection<Feature> HumainFeatures() =>
  [
    new(new Name("Apprentissage accéléré"), Content.TryCreate("Le personnage débute avec 4 points d’[Apprentissage](/regles/statistiques/apprentissage) supplémentaires. Il acquiert également 1 point d’Apprentissage supplémentaire chaque fois que son [tiers](/regles/personnages/progression/tiers) augmente.")),
    new(new Name("Aspect"), Content.TryCreate("Le personnage acquiert gratuitement le talent [Entraînement I](/regles/talents/entrainement-i).")),
    new(new Name("Versatilité"), Content.TryCreate("Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) deux [talents](/regles/talents) le [formant](/regles/competences/formation) pour une [compétence](/regles/competences)."))
  ];

  public static IReadOnlyCollection<Feature> ElfeFeatures() =>
  [
    new(new Name("Esprit éveillé"), Content.TryCreate("Le personnage ne peut être endormi de manière surnaturelle et il se voit conférer l’[avantage](/regles/competences/tests/avantage-desavantage) à ses [jets de sauvegarde](/regles/competences/tests/sauvegarde) contre les [charmes](/regles/combat/conditions/charme). Il peut également s’[harmoniser](/regles/magie/artefacts/harmonisation) à un [artefact magique](/regles/magie/artefacts) supplémentaire.")),
    new(new Name("Sens affûtés"), Content.TryCreate("Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) les talents [Orientation](/regles/talents/orientation) et [Perception](/regles/talents/perception).")),
    new(new Name("Transe"), Content.TryCreate("Le personnage peut remplacer la [nuit de sommeil](/regles/aventure/repos/sommeil) conventionnelle de [8 heures](/regles/aventure/temps) par une transe d’une durée de seulement 4 heures. Compléter cette transe procure les mêmes effets que de compléter une nuit de sommeil. Pendant cette transe, le personnage est en état de conscience partielle, mélangeant rêves éveillés et lucidité détachée. Il demeure partiellement réceptif à son [environnement](/regles/aventure/environnement) et peut effectuer avec [désavantage](/regles/competences/tests/avantage-desavantage) des [tests passifs](/regles/competences/tests/passif)."))
  ];

  public static IReadOnlyCollection<Feature> NainFeatures() =>
  [
    new(new Name("Débrouillard"), Content.TryCreate("Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) le talent [Artisanat](/regles/talents/artisanat).")),
    new(new Name("Épaules larges"), Content.TryCreate("Le personnage se voit conférer un bonus permanent (50 %) à sa [Charge](/regles/statistiques/charge).")),
    new(new Name("Métabolisme nain"), Content.TryCreate("Le personnage se voit conférer un bonus à son [seuil de tolérance à l’alcool](/regles/aventure/environnement/alcoolemie) égal à son [tiers](/regles/personnages/progression/tiers) (minimum 1).")),
    new(new Name("Vision nocturne"), Content.TryCreate("Le personnage acquiert une [vision dans le noir](/regles/aventure/environnement/vision) à une distance de 18 mètres."))
  ];
}
