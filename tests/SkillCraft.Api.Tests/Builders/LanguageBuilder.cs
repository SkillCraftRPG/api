using Bogus;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ILanguageBuilder
{
  ILanguageBuilder WithId(Guid id);
  ILanguageBuilder WithWorld(World? world);
  ILanguageBuilder WithName(string name);
  ILanguageBuilder WithSummary(string? summary);
  ILanguageBuilder WithHtmlContent(string? htmlContent);
  ILanguageBuilder WithScript(Script? script);
  ILanguageBuilder WithTypicalSpeakers(string? typicalSpeakers);

  Language Build();
}

public class LanguageBuilder : ILanguageBuilder
{
  private readonly Faker _faker;

  private string? _htmlContent = null;
  private Guid? _id = null;
  private string _name = "Language";
  private Script? _script = null;
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

  public ILanguageBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
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
    return new Language(world, _name, _id, _summary, _htmlContent, _script, _typicalSpeakers);
  }

  public static Language Common(Faker? faker = null, World? world = null, Script? script = null) => new LanguageBuilder(faker)
    .WithWorld(world)
    .WithName("Commun")
    .WithSummary("Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.")
    .WithHtmlContent("Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.")
    .WithScript(script ?? ScriptBuilder.Renon(faker, world))
    .WithTypicalSpeakers("Humains")
    .Build();
}
