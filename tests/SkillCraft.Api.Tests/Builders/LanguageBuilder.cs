using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ILanguageBuilder
{
  ILanguageBuilder WithId(LanguageId languageId);
  ILanguageBuilder WithWorld(World? world);
  ILanguageBuilder WithName(string name);
  ILanguageBuilder WithSummary(string? summary);
  ILanguageBuilder WithContent(string? content);
  ILanguageBuilder WithScript(Script? script);
  ILanguageBuilder WithTypicalSpeakers(string? typicalSpeakers);

  Language Build();
}

public class LanguageBuilder : ILanguageBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private LanguageId? _languageId = null;
  private string _name = "Language";
  private Script? _script = null;
  private string? _summary = null;
  private string? _typicalSpeakers = null;
  private World? _world = null;

  public LanguageBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ILanguageBuilder WithId(LanguageId languageId)
  {
    _languageId = languageId;
    return this;
  }

  public ILanguageBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ILanguageBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ILanguageBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ILanguageBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public ILanguageBuilder WithScript(Script? script)
  {
    _script = script;
    return this;
  }

  public ILanguageBuilder WithTypicalSpeakers(string? typicalSpeakers)
  {
    _typicalSpeakers = typicalSpeakers;
    return this;
  }

  public Language Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    ActorId actorId = world.OwnerId.ActorId;
    Name name = new(_name);

    Language language = _languageId.HasValue
      ? new(_languageId.Value, name, actorId)
      : new(world, name, actorId);

    language.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);
    language.SetRules(TypicalSpeakers.TryCreate(_typicalSpeakers), _script, actorId);

    return language;
  }

  public static Language Common(Faker? faker = null, World? world = null, Script? script = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Commun")
    .WithSummary("Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.")
    .WithContent("Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.")
    .WithScript(script)
    .WithTypicalSpeakers("Humains")
    .Build();

  public static Language Celfique(Faker? faker = null, World? world = null, Script? script = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Celfique")
    .WithSummary("Langue elfique majeure, traditions, dialectes et identité culturelle.")
    .WithContent("Le Celfique est la langue commune de la majorité des [peuples elfiques](/regles/especes/elfe) d’Ouespéro. Elle sert à la fois de langue du quotidien, de tradition et d’identité, avec une sonorité réputée fluide et un vocabulaire très précis pour tout ce qui touche aux forêts, aux reliefs et aux cycles naturels. Hors des communautés elfiques, elle est surtout perçue comme une langue de connivence, car les elfes l’emploient volontiers entre eux pour converser sans être compris, ce qui agace fréquemment leurs voisins.\n\nLe Celfique et le [Sylvestre](/regles/langues/sylvestre) appartiennent à la famille glaïdique, dont le nom signifie littéralement _« les parlers des bois »_. Ces deux langues descendent d’un ancêtre commun ancien, mais ont suivi des évolutions distinctes. Bien qu’elles partagent encore certaines structures et racines, elles sont aujourd’hui suffisamment divergentes pour ne plus être mutuellement intelligibles.")
    .WithScript(script)
    .WithTypicalSpeakers("Hauts-Elfes, Elfes des mers, Elfes cramoisis et Elfes sylvains")
    .Build();

  public static Language Sylvestre(Faker? faker = null, World? world = null, Script? script = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Sylvestre")
    .WithSummary("Langue elfique ancienne, rituelle et identitaire des peuples sylvains.")
    .WithContent("Le Sylvestre est une langue elfique ancienne appartenant à la famille glaïdique, un ensemble linguistique dont le nom signifie communément _« les parlers des bois »_. Issue d’un ancêtre commun partagé avec le [Celfique](/regles/langues/celfique), la langue Sylvestre a suivi une évolution distincte, conservant une forte cohérence interne tout en s’éloignant progressivement de ses parentes continentales.\n\nLe Sylvestre se caractérise par une morphologie riche, une syntaxe souple et une forte musicalité, particulièrement adaptée aux récits oraux, aux chants rituels et aux traditions mémorielles. Bien que rarement enseigné hors des communautés concernées, le Sylvestre reste l’un des piliers linguistiques du monde glaïdique, transmis avec soin par tradition orale et écrite.\n\nLe Sylvestre est une langue chantée plus que parlée : le rythme et l’intonation comptent autant que les mots eux-mêmes. Les noms de lieux, d’arbres et de rivières sont considérés comme anciens et vivants. Mal les prononcer est vu comme une insulte envers la forêt. Elle est naturellement appropriée aux récits, aux serments et aux pactes anciens.")
    .WithScript(script)
    .WithTypicalSpeakers("Le Sylvestre est parlé principalement dans les Triskîles, où il est la langue dominante des [Elfes sylvains](/regles/especes/elfe/sylvain), mais aussi de nombreuses fées, notamment les [Nemediens](/regles/especes/nemedien) et les [Fir Bolg](/regles/especes/fir-bolg). Il sert autant de langue quotidienne que de langue cérémonielle, et demeure volontairement peu partagé avec les peuples non glaïdiques, ce qui renforce son rôle identitaire et culturel.")
    .Build();
}
