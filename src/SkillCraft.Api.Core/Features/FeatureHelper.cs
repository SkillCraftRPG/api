namespace SkillCraft.Api.Core.Features;

public static class FeatureHelper
{
  public static Feature Create(string name, string? content = null) => new(new Name(name), Content.TryCreate(content));
}
