using Bogus;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IItemBuilder
{
  IItemBuilder WithId(Guid id);
  IItemBuilder WithWorld(World? world);
  IItemBuilder WithName(string name);
  IItemBuilder WithSummary(string? summary);
  IItemBuilder WithContent(string? content);
  IItemBuilder WithPrice(double? price);
  IItemBuilder WithWeight(double? weight);

  Item Build();
}

public class ItemBuilder : IItemBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private Guid? _id = null;
  private string _name = "Item";
  private double? _price = null;
  private string? _summary = null;
  private double? _weight = null;
  private World? _world = null;

  public ItemBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IItemBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public IItemBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public IItemBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public IItemBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public IItemBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public IItemBuilder WithPrice(double? price)
  {
    _price = price;
    return this;
  }

  public IItemBuilder WithWeight(double? weight)
  {
    _weight = weight;
    return this;
  }

  public Item Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    Item item = new(world, _id)
    {
      Name = _name,
      Summary = _summary,
      Content = _content,
      Price = _price,
      Weight = _weight
    };
    return item;
  }

  public static Item Abaque(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Abaque")
    .WithSummary("Outil de compte en bois.")
    .WithContent("Permet de faire des calculs arithmétiques simples. Un boîtier de bois doté de tiges autour desquelles se trouvent de petits jetons.")
    .WithPrice(2)
    .WithWeight(1)
    .Build();

  public static Item Corde(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Corde (15 mètres)")
    .WithSummary("Corde de chanvre de 15 mètres, 2 points de Vitalité.")
    .WithContent("Une corde de chanvre dotée de 2 points de Vitalité. On peut la briser en réussissant un test d’Athlétisme de difficulté élevée. La longueur standard est de 15 mètres.")
    .WithPrice(1)
    .WithWeight(5)
    .Build();

  public static Item Lanterne(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Lanterne")
    .WithSummary("Lampe à huile avec capuchon, clarté 9 m / pénombre 9 m.")
    .WithContent("Lorsqu’elle est allumée, elle émet de la clarté dans un rayon de 9 mètres et de la pénombre dans un rayon supplémentaire de 9 mètres. Elle est dotée d’un capuchon permettant de réduire sa lumière à un rayon de 1,5 mètres de pénombre. Elle consomme environ 2 onces d’huile (62,5 ml) par heure, soit une flasque (500 ml) pour 8 heures.")
    .WithPrice(5)
    .WithWeight(1)
    .Build();

  public static Item Torche(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Torche")
    .WithSummary("Source de lumière d’une heure, utilisable comme arme improvisée.")
    .WithContent("Lorsqu’elle est allumée, elle émet de la clarté dans un rayon de 6 mètres et de la pénombre dans un rayon supplémentaire de 6 mètres pendant une heure. On peut l’utiliser comme arme improvisée afin d’infliger 1 point de dégâts de feu.")
    .WithPrice(0.01)
    .WithWeight(0.5)
    .Build();

  public static Item PiedDeBiche(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Pied de biche")
    .WithSummary("Outil de levier conférant l’avantage aux tests d’Athlétisme.")
    .WithContent("Confère l’avantage aux tests d’Athlétisme pour forcer quelque chose avec l’effet de levier.")
    .WithPrice(2)
    .WithWeight(2.5)
    .Build();

  public static Item Grimoire(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Grimoire")
    .WithSummary("Tome de 100 pages nécessaire aux Astromanciens.")
    .WithContent("Un tome de 100 pages reliées par une couverture de cuir nécessaire aux Astromanciens.")
    .WithPrice(50)
    .WithWeight(1.5)
    .Build();
}
