using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ICustomizationBuilder
{
  ICustomizationBuilder WithId(CustomizationId customizationId);
  ICustomizationBuilder WithWorld(World? world);
  ICustomizationBuilder WithKind(CustomizationKind kind);
  ICustomizationBuilder WithName(string name);
  ICustomizationBuilder WithSummary(string? summary);
  ICustomizationBuilder WithContent(string? content);

  Customization Build();
}

public class CustomizationBuilder : ICustomizationBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private CustomizationId? _customizationId = null;
  private CustomizationKind? _kind = null;
  private string _name = "Customization";
  private string? _summary = null;
  private World? _world = null;

  public CustomizationBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ICustomizationBuilder WithId(CustomizationId customizationId)
  {
    _customizationId = customizationId;
    return this;
  }

  public ICustomizationBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ICustomizationBuilder WithKind(CustomizationKind kind)
  {
    _kind = kind;
    return this;
  }

  public ICustomizationBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ICustomizationBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ICustomizationBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public Customization Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    ActorId actorId = world.OwnerId.ActorId;
    CustomizationKind kind = _kind ?? _faker.PickRandom(CustomizationKind.Disability, CustomizationKind.Gift);
    Name name = new(_name);

    Customization customization = _customizationId.HasValue
      ? new(_customizationId.Value, kind, name, actorId)
      : new(world, kind, name, actorId);

    customization.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);

    return customization;
  }

  public static Customization Fignolage(Faker? faker = null, World? world = null) => new CustomizationBuilder(faker)
    .WithWorld(world)
    .WithKind(CustomizationKind.Gift)
    .WithName("Fignolage")
    .WithSummary("Permet de perfectionner ses créations et réparer sous pression.")
    .WithContent("Le personnage acquiert les capacités suivantes :\n\n- Il peut [Faire 20](/regles/competences/tests/faire-20) à un [test](/regles/competences/tests) d’[Artisanat](/regles/competences/artisanat) lorsqu’il n’est pas [menacé](/regles/combat), qu’il n’est pas distrait, et qu’il n’est pas contraint par le [temps](/regles/aventure/temps).\n- Il peut dépenser 15 points d’[Énergie](/regles/statistiques/energie) afin de conférer l’[avantage](/regles/competences/tests/avantage-desavantage) à un de ses tests d’Artisanat.\n- Il peut [Faire 10](/regles/competences/tests/faire-10) lorsqu’il utilise une [trousse de réparation](/regles/equipement/outils) pour [réparer un objet](/regles/talents/artisanat), même lorsqu’il est menacé ou distrait.")
    .Build();

  public static Customization Hemophobe(Faker? faker = null, World? world = null) => new CustomizationBuilder(faker)
    .WithWorld(world)
    .WithKind(CustomizationKind.Disability)
    .WithName("Hémophobe")
    .WithSummary("Panique à la vue du sang, provoquant peur et perte de contrôle.")
    .WithContent("Peu n’effraie davantage le personnage que la vue du sang ou d’une plaie ouverte.\n\nLorsque lui-même ou un allié situé à 1,5 mètres ou moins de sa position est blessé par des [points de dégâts](/regles/combat/degats) [tranchants ou perforants](/regles/combat/degats/types), alors il doit effectuer un [jet de sauvegarde](/regles/competences/tests/sauvegarde) de [Discipline](/regles/competences/discipline) de [difficulté](/regles/competences/tests/difficulte) égale aux points de dégâts subis.\n\nEn cas d’échec, il est [apeuré](/regles/combat/conditions/apeure) jusqu’à la fin de son prochain [tour](/regles/combat/deroulement/tour).")
    .Build();
}
