using Bogus;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ITalentBuilder
{
  ITalentBuilder WithId(Guid id);
  ITalentBuilder WithWorld(World? world);
  ITalentBuilder WithTier(int tier);
  ITalentBuilder WithName(string name);
  ITalentBuilder WithSummary(string? summary);
  ITalentBuilder WithHtmlContent(string? htmlContent);
  ITalentBuilder WithAllowMultiplePurchases(bool allowMultiplePurchases);
  ITalentBuilder WithSkill(Skill? skill);
  ITalentBuilder WithRequiredTalent(Talent? requiredTalent);

  Talent Build();
}

public class TalentBuilder : ITalentBuilder
{
  private readonly Faker _faker;

  private bool _allowMultiplePurchases = false;
  private string? _htmlContent = null;
  private Guid? _id = null;
  private string _name = "Talent";
  private Talent? _requiredTalent = null;
  private Skill? _skill = null;
  private string? _summary = null;
  private int _tier = 0;
  private World? _world = null;

  public TalentBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ITalentBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public ITalentBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ITalentBuilder WithTier(int tier)
  {
    _tier = tier;
    return this;
  }

  public ITalentBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ITalentBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ITalentBuilder WithHtmlContent(string? htmlContent)
  {
    _htmlContent = htmlContent;
    return this;
  }

  public ITalentBuilder WithAllowMultiplePurchases(bool allowMultiplePurchases)
  {
    _allowMultiplePurchases = allowMultiplePurchases;
    return this;
  }

  public ITalentBuilder WithSkill(Skill? skill)
  {
    _skill = skill;
    return this;
  }

  public ITalentBuilder WithRequiredTalent(Talent? requiredTalent)
  {
    _requiredTalent = requiredTalent;
    return this;
  }

  public Talent Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Talent(world, _tier, _name, _id, _summary, _htmlContent, _allowMultiplePurchases, _skill, _requiredTalent);
  }
}
