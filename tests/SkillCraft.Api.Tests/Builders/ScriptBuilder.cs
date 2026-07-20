using Bogus;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IScriptBuilder
{
  IScriptBuilder WithId(Guid id);
  IScriptBuilder WithWorld(World? world);
  IScriptBuilder WithName(string name);
  IScriptBuilder WithDescription(string? description);

  Script Build();
}

public class ScriptBuilder : IScriptBuilder
{
  private readonly Faker _faker;

  private string? _description = null;
  private Guid? _id = null;
  private string _name = "Script";
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

  public IScriptBuilder WithDescription(string? description)
  {
    _description = description;
    return this;
  }

  public Script Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Script(world, _name, _id, _description);
  }

  public static Script Renon(Faker? faker = null, World? world = null) => new ScriptBuilder(faker)
    .WithWorld(world)
    .WithName("Rénon")
    .WithDescription("Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.")
    .Build();
}
