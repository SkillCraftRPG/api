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
}
