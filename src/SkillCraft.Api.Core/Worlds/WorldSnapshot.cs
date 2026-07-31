using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Core.Worlds;

public record WorldSnapshot
{
  public string Key { get; }
  public string? Name { get; }
  public string? HtmlContent { get; }

  public WorldSnapshot(World world)
  {
    Key = world.Key;
    Name = world.Name;
    HtmlContent = world.HtmlContent;
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

    if (HtmlContent != world.HtmlContent)
    {
      changes++;
      record.HtmlContent = new Change<string>(HtmlContent, world.HtmlContent);
    }

    return changes < 1 ? null : record;
  }
}
