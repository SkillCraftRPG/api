using Bogus;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ICustomizationBuilder
{
  ICustomizationBuilder WithId(Guid id);
  ICustomizationBuilder WithWorld(World? world);
  ICustomizationBuilder WithKind(CustomizationKind kind);
  ICustomizationBuilder WithName(string name);
  ICustomizationBuilder WithSummary(string? summary);
  ICustomizationBuilder WithHtmlContent(string? htmlContent);

  Customization Build();
}

public class CustomizationBuilder : ICustomizationBuilder
{
  private readonly Faker _faker;

  private string? _htmlContent = null;
  private Guid? _id = null;
  private CustomizationKind? _kind = null;
  private string _name = "Customization";
  private string? _summary = null;
  private World? _world = null;

  public CustomizationBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ICustomizationBuilder WithId(Guid id)
  {
    _id = id;
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

  public ICustomizationBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
    return this;
  }

  public Customization Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    CustomizationKind kind = _kind ?? _faker.PickRandom<CustomizationKind>();
    return new Customization(world, kind, _name, _id, _summary, _htmlContent);
  }
}
