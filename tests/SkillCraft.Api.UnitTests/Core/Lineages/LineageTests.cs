using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

[Trait(Traits.Category, Categories.Unit)]
public class LineageTests
{
  private readonly UnitTestContext _context = UnitTestContext.Generate();

  [Fact(DisplayName = "It should handle Features changes correctly.")]
  public void Given_Changes_When_SetFeatures_Then_HandledCorrectly()
  {
    Feature[] features =
    [
      new Feature(new Name("Camouflage naturel"), Description:null),
      new Feature(new Name("Esprit éveillé"), new Description("Le personnage ne peut être endormi de manière surnaturelle.\n\nÉgalement, il peut remplacer ses [6 heures](/regles/aventure/temps) de sommeil quotidien par 4 heures de transe.\n\nEn plus d’écourter sa [nuit de sommeil](/regles/aventure/repos/sommeil), il est en état de conscience partielle pendant cette transe, ce qui lui permet d’effectuer avec [désavantage](/regles/competences/tests/avantage-desavantage) des [tests passifs](/regles/competences/tests/passif).\n\nIl doit tout de même jumeler cette transe avec 2 heures de [halte](/regles/aventure/repos/halte) afin de compléter sa nuit de sommeil.")),
      new Feature(new Name("Sens affûtés"), new Description("Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) les talents [Orientation](/regles/talents/orientation) et [Perception](/regles/talents/perception).")),
      new Feature(new Name("Camouflage naturel"), new Description("Le personnage peut tenter de [se cacher](/regles/combat/activites/cacher) d’une créature qui le voit lorsqu’il est [légèrement obscurci](/regles/aventure/environnement/vision) par un phénomène naturel tel du feuillage, de la pluie battante, une tempête de neige ou du brouillard.")),
    ];

    Lineage lineage = new(_context.World, new Name("Elfe"));
    lineage.SetFeatures(features);
    lineage.Update(_context.UserId);
    Assert.True(lineage.HasChanges);

    lineage.ClearChanges();
    Assert.False(lineage.HasChanges);

    lineage.SetFeatures(features);
    lineage.Update(_context.UserId);
    Assert.False(lineage.HasChanges);
  }

  [Fact(DisplayName = "It should handle Languages changes correctly.")]
  public void Given_Changes_When_SetLanguages_Then_HandledCorrectly()
  {
    Language celfique = new(_context.World, new Name("Celfique"));
    Language commun = new(_context.World, new Name("Commun"));

    Lineage lineage = new(_context.World, new Name("Elfe"));
    lineage.SetLanguages([celfique, commun], extra: 1, new Description("Let’s learn some languages!"));
    lineage.Update(_context.UserId);
    Assert.True(lineage.HasChanges);

    lineage.ClearChanges();
    Assert.False(lineage.HasChanges);

    lineage.SetLanguages([celfique, commun], extra: 1, new Description("Let’s learn some languages!"));
    lineage.Update(_context.UserId);
    Assert.False(lineage.HasChanges);
  }

  [Fact(DisplayName = "It should handle Names changes correctly.")]
  public void Given_Changes_When_Names_Then_HandledCorrectly()
  {
    Names names = new(
      ["Adlegor", "Charimon", "Galanodel", "Kaendere", "Liadon", "Morwen", "Nodir", "Raelden", "Sonomir", "Talath"],
      ["Althaea", "Ceanoise", "Elenna", "Leshanna", "Magilin", "Naeva", "Onoraid", "Sariel", "Tibenna", "Valanthe"],
      ["Aelar", "Berrian", "Erevan", "Feirion", "Hononn", "Ivellios", "Kaimear", "Peren", "Saemainn", "Therion"],
      [],
      new Dictionary<string, IReadOnlyCollection<string>>());

    Lineage lineage = new(_context.World, new Name("Elfe"));
    lineage.Names = names;
    lineage.Update(_context.UserId);
    Assert.True(lineage.HasChanges);

    lineage.ClearChanges();
    Assert.False(lineage.HasChanges);

    lineage.Names = names;
    lineage.Update(_context.UserId);
    Assert.False(lineage.HasChanges);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the parent is from a different world.")]
  public void Given_ParentFromDifferentWorld_When_ctor_Then_ArgumentException()
  {
    Lineage parent = new(WorldId.NewId(), new Name("Elfe"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => new Lineage(_context.World, new Name("Elfe sylvain"), parent));

    Assert.Equal("parent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a language is from a different world.")]
  public void Given_LanguageFromDifferentWorld_When_SetLanguages_Then_ArgumentException()
  {
    Lineage lineage = new(_context.World, new Name("Lineage"));
    Language language = new(WorldId.NewId(), new Name("Celfique"), _context.UserId);

    var exception = Assert.Throws<ArgumentException>(() => lineage.SetLanguages([language]));

    Assert.Equal("languages", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw InvalidParentLineageException when the parent has a parent.")]
  public void Given_ParentWithParent_When_ctor_Then_InvalidParentLineageException()
  {
    Lineage parent = new(_context.World, new Name("Elfe"));
    Lineage lineage = new(_context.World, new Name("Elfe sylvain"), parent);

    LineageId lineageId = LineageId.NewId(lineage.WorldId);
    var exception = Assert.Throws<InvalidParentLineageException>(() => new Lineage(_context.World, new Name("Elfes sylvains de Milande"), lineage, _context.UserId, lineageId));

    Assert.Equal(lineage.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(lineageId.EntityId, exception.LineageId);
    Assert.Equal(lineage.EntityId, exception.ParentId);
    Assert.Equal(parent.EntityId, exception.AncestorId);
    Assert.Equal("ParentId", exception.PropertyName);
  }
}
