using Bogus;
using Krakenar.Contracts.Users;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface IWorldBuilder
{
  IWorldBuilder WithId(Guid id);
  IWorldBuilder WithOwner(User? owner);
  IWorldBuilder WithKey(string key);
  IWorldBuilder WithName(string? name);
  IWorldBuilder WithHtmlContent(string? htmlContent);

  World Build();
}

public class WorldBuilder : IWorldBuilder
{
  private readonly Faker _faker;

  private string? _htmlContent = null;
  private Guid? _id = null;
  private string _key = "ungar";
  private string? _name = "Ungar";
  private User? _owner = null;

  public WorldBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IWorldBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public IWorldBuilder WithOwner(User? owner)
  {
    _owner = owner;
    return this;
  }

  public IWorldBuilder WithKey(string key)
  {
    _key = key;
    return this;
  }

  public IWorldBuilder WithName(string? name)
  {
    _name = name;
    return this;
  }

  public IWorldBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
    return this;
  }

  public World Build()
  {
    User owner = _owner ?? new UserBuilder(_faker).Build();
    return new World(owner.Id, _key, _id, _name, _htmlContent);
  }
}
