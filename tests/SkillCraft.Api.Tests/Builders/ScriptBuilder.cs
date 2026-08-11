using Bogus;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IScriptBuilder
{
  IScriptBuilder WithId(ScriptId scriptId);
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
  private string _name = "Script";
  private ScriptId? _scriptId = null;
  private string? _summary = null;
  private World? _world = null;

  public ScriptBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IScriptBuilder WithId(ScriptId scriptId)
  {
    _scriptId = scriptId;
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
    ActorId actorId = world.OwnerId.ActorId;
    Name name = new(_name);

    Script script = _scriptId.HasValue
      ? new(_scriptId.Value, name, actorId)
      : new(world, name, actorId);

    script.Edit(Summary.TryCreate(_summary), Content.TryCreate(_content), actorId);

    return script;
  }

  public static Script Elfique(Faker? faker = null, World? world = null) => new ScriptBuilder(faker)
    .WithWorld(world)
    .WithName("Elfique")
    .WithSummary("Alphabet ancien des Elfes, utilisé pour les langues, rituels et archives.")
    .WithContent("L’alphabet Elfique est un système d’écriture ancien, créé par les Elfes il y a plus de 5 000 ans. Il est principalement utilisé pour transcrire les langues elfiques et le [Sylvestre](/regles/langues/sylvestre), mais on le retrouve aussi dans des inscriptions rituelles, des marqueurs de frontière et des archives nobles. C’est un système majoritairement phonémique, écrit surtout de droite à gauche, dont les lettres linéaires s’organisent autour d’un axe central, avec des voyelles marquées par la position et la courbure, une écriture souvent liée, une ponctuation simple et des chiffres fondés sur des entailles de comptage. Il est employé aussi bien dans des contextes quotidiens que monumentaux ou savants, et se décline en plusieurs variantes graphiques (courante, monumentale et sacrée) adaptées à la vitesse d’écriture, au support et au caractère solennel du texte.")
    .Build();

  public static Script Renon(Faker? faker = null, World? world = null) => new ScriptBuilder(faker)
    .WithWorld(world)
    .WithName("Rénon")
    .WithSummary("Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.")
    .WithContent("L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.")
    .Build();
}
