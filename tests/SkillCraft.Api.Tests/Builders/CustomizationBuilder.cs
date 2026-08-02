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
}
