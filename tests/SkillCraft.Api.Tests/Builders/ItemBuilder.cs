using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IItemBuilder
{
  IItemBuilder WithId(ItemId itemId);
  IItemBuilder WithWorld(World? world);
  IItemBuilder WithCategory(ItemCategory category);
  IItemBuilder WithName(string name);
  IItemBuilder WithSummary(string? summary);
  IItemBuilder WithContent(string? content);
  IItemBuilder WithPrice(int? price);
  IItemBuilder WithWeight(int? weight);
  IItemBuilder WithRarity(ItemRarity? rarity);
  IItemBuilder WithCharges(ItemCharges? charges);

  Item Build();
}

public class ItemBuilder : IItemBuilder
{
  private readonly Faker _faker;

  private ItemCharges? _charges = null;
  private ItemCategory _category = ItemCategory.Miscellaneous;
  private string? _content = null;
  private ItemId? _itemId = null;
  private string _name = "Item";
  private int? _price = null;
  private ItemRarity? _rarity = null;
  private string? _summary = null;
  private int? _weight = null;
  private World? _world = null;

  public ItemBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IItemBuilder WithId(ItemId itemId)
  {
    _itemId = itemId;
    return this;
  }

  public IItemBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public IItemBuilder WithCategory(ItemCategory category)
  {
    _category = category;
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

  public IItemBuilder WithPrice(int? price)
  {
    _price = price;
    return this;
  }

  public IItemBuilder WithWeight(int? weight)
  {
    _weight = weight;
    return this;
  }

  public IItemBuilder WithRarity(ItemRarity? rarity)
  {
    _rarity = rarity;
    return this;
  }

  public IItemBuilder WithCharges(ItemCharges? charges)
  {
    _charges = charges;
    return this;
  }

  public Item Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    ActorId actorId = world.OwnerId.ActorId;
    Name name = new(_name);

    Item item = _itemId.HasValue
      ? new(_itemId.Value, _category, name, actorId)
      : new(world, _category, name, actorId);

    item.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);
    item.SetRules(Price.TryCreate(_price), Weight.TryCreate(_weight), _rarity, _charges, actorId);

    return item;
  }

  public static Item Abaque(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Abaque")
    .WithSummary("Outil de compte en bois.")
    .WithContent("Permet de faire des calculs arithmétiques simples. Un boîtier de bois doté de tiges autour desquelles se trouvent de petits jetons.")
    .WithPrice(200)
    .WithWeight(100)
    .Build();

  public static Item Corde(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Corde (15 mètres)")
    .WithSummary("Corde de chanvre de 15 mètres, 2 points de Vitalité.")
    .WithContent("Une corde de chanvre dotée de 2 points de Vitalité. On peut la briser en réussissant un test d’Athlétisme de difficulté élevée. La longueur standard est de 15 mètres.")
    .WithPrice(100)
    .WithWeight(500)
    .Build();

  public static Item Denier(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithCategory(ItemCategory.Currency)
    .WithName("Denier")
    .WithSummary("Pièce de valeur 1 en argent.")
    .WithContent("Une pièce d’argent couramment utilisée par la plupart des membres de la société pour leurs achats de plus grande valeur, comme le bétail de moyens de transport. Il s’agit de l’unité de référence de ce système.\r\n")
    .WithPrice(100)
    .WithWeight(1)
    .Build();

  public static Item Fiole(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithCategory(ItemCategory.Container)
    .WithName("Fiole")
    .WithSummary("Petite fiole en verre de 120 ml.")
    .WithContent("Une petite fiole en verre dotée d’un petit bouchon de liège.")
    .WithPrice(1)
    .Build();

  public static Item Grimoire(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Grimoire")
    .WithSummary("Tome de 100 pages nécessaire aux Astromanciens.")
    .WithContent("Un tome de 100 pages reliées par une couverture de cuir nécessaire aux Astromanciens.")
    .WithPrice(5000)
    .WithWeight(150)
    .Build();

  public static Item Lanterne(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Lanterne")
    .WithSummary("Lampe à huile avec capuchon, clarté 9 m / pénombre 9 m.")
    .WithContent("Lorsqu’elle est allumée, elle émet de la clarté dans un rayon de 9 mètres et de la pénombre dans un rayon supplémentaire de 9 mètres. Elle est dotée d’un capuchon permettant de réduire sa lumière à un rayon de 1,5 mètres de pénombre. Elle consomme environ 2 onces d’huile (62,5 ml) par heure, soit une flasque (500 ml) pour 8 heures.")
    .WithPrice(500)
    .WithWeight(100)
    .Build();

  public static Item PiedDeBiche(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Pied de biche")
    .WithSummary("Outil de levier conférant l’avantage aux tests d’Athlétisme.")
    .WithContent("Confère l’avantage aux tests d’Athlétisme pour forcer quelque chose avec l’effet de levier.")
    .WithPrice(200)
    .WithWeight(250)
    .Build();

  public static Item Torche(Faker? faker = null, World? world = null) => new ItemBuilder(faker)
    .WithWorld(world)
    .WithName("Torche")
    .WithSummary("Source de lumière d’une heure, utilisable comme arme improvisée.")
    .WithContent("Lorsqu’elle est allumée, elle émet de la clarté dans un rayon de 6 mètres et de la pénombre dans un rayon supplémentaire de 6 mètres pendant une heure. On peut l’utiliser comme arme improvisée afin d’infliger 1 point de dégâts de feu.")
    .WithPrice(1)
    .WithWeight(50)
    .Build();
}
