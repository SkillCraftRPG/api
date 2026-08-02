using Bogus;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ILanguageBuilder
{
  ILanguageBuilder WithId(Guid id);
  ILanguageBuilder WithWorld(World? world);
  ILanguageBuilder WithName(string name);
  ILanguageBuilder WithSummary(string? summary);
  ILanguageBuilder WithContent(string? content);
  ILanguageBuilder WithScript(int? scriptId, Guid? scriptUid = null);
  ILanguageBuilder WithTypicalSpeakers(string? typicalSpeakers);

  Language Build();
}

public class LanguageBuilder : ILanguageBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private Guid? _id = null;
  private string _name = "Language";
  private int? _scriptId = null;
  private Guid? _scriptUid = null;
  private string? _summary = null;
  private string? _typicalSpeakers = null;
  private World? _world = null;

  public LanguageBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ILanguageBuilder WithId(Guid id)
  {
    _id = id;
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

  public ILanguageBuilder WithScript(int? scriptId, Guid? scriptUid = null)
  {
    _scriptId = scriptId;
    _scriptUid = scriptUid;
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
    Language language = new(world, _id)
    {
      Name = _name,
      Summary = _summary,
      Content = _content,
      TypicalSpeakers = _typicalSpeakers
    };
    language.SetScript(_scriptId, _scriptUid);
    return language;
  }

  public static Language Common(Faker? faker = null, World? world = null, int? scriptId = null, Guid? scriptUid = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Commun")
    .WithSummary("Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.")
    .WithContent("Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.")
    .WithScript(scriptId, scriptUid)
    .WithTypicalSpeakers("Humains")
    .Build();

  public static Language Celfique(Faker? faker = null, World? world = null, int? scriptId = null, Guid? scriptUid = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Celfique")
    .WithSummary("Langue elfique majeure, traditions, dialectes et identité culturelle.")
    .WithContent("Le Celfique est la langue commune de la majorité des [peuples elfiques](/regles/especes/elfe) d’Ouespéro. Elle sert à la fois de langue du quotidien, de tradition et d’identité, avec une sonorité réputée fluide et un vocabulaire très précis pour tout ce qui touche aux forêts, aux reliefs et aux cycles naturels. Hors des communautés elfiques, elle est surtout perçue comme une langue de connivence, car les elfes l’emploient volontiers entre eux pour converser sans être compris, ce qui agace fréquemment leurs voisins.\n\nLe Celfique et le [Sylvestre](/regles/langues/sylvestre) appartiennent à la famille glaïdique, dont le nom signifie littéralement _« les parlers des bois »_. Ces deux langues descendent d’un ancêtre commun ancien, mais ont suivi des évolutions distinctes. Bien qu’elles partagent encore certaines structures et racines, elles sont aujourd’hui suffisamment divergentes pour ne plus être mutuellement intelligibles.")
    .WithScript(scriptId, scriptUid)
    .WithTypicalSpeakers("Hauts-Elfes, Elfes des mers, Elfes cramoisis et Elfes sylvains")
    .Build();
}
