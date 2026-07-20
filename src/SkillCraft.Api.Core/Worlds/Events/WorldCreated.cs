namespace SkillCraft.Api.Core.Worlds.Events;

public class WorldCreated : CreateEvent
{
  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? HtmlContent { get; set; }

  public WorldCreated() : base()
  {
  }

  public WorldCreated(World world) : base(world)
  {
    Key = world.Key;
    Name = world.Name;
    HtmlContent = world.HtmlContent;
  }
}
