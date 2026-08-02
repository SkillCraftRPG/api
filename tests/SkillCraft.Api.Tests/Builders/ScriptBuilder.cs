using Bogus;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IScriptBuilder
{
  IScriptBuilder WithId(Guid id);
  IScriptBuilder WithWorld(World? world);
  IScriptBuilder WithName(string name);
  IScriptBuilder WithSummary(string? summary);
  IScriptBuilder WithContent(string? content);

  Script Build();
}

public class ScriptBuilder : IScriptBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private Guid? _id = null;
  private string _name = "Script";
  private string? _summary = null;
  private World? _world = null;

  public ScriptBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IScriptBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public IScriptBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public IScriptBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public IScriptBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public IScriptBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public Script Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Script(world, _id)
    {
      Name = _name,
      Summary = _summary,
      Content = _content
    };
  }

  public static Script Renon(Faker? faker = null, World? world = null) => new ScriptBuilder(faker)
    .WithWorld(world)
    .WithName("Rénon")
    .WithSummary("Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.")
    .WithContent("L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.")
    .Build();
}
