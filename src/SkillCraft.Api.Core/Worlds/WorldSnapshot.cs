using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Core.Worlds;

public record WorldSnapshot
{
  public string Key { get; }
  public string? Name { get; }
  public string? Content { get; }

  public WorldSnapshot(World world)
  {
    Key = world.Key;
    Name = world.Name;
    Content = world.Content;
  }

  public WorldUpdated? Compare(World world)
  {
    int changes = 0;
    WorldUpdated record = new(world);

    if (Key != world.Key)
    {
      changes++;
      record.Key = new Change<string>(Key, world.Key);
    }

    if (Name != world.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, world.Name);
    }

    if (Content != world.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, world.Content);
    }

    return changes < 1 ? null : record;
  }
}
