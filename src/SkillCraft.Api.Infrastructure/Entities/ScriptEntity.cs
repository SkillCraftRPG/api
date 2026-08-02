using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class ScriptEntity : AggregateEntity
{
  public int ScriptId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public List<Language> Languages { get; private set; } = [];

  public ScriptEntity(Script script) : base(script)
  {
    WorldId = script.WorldId.ResourceId;
    Id = script.ResourceId;

    Update(script);
  }

  public ScriptEntity(ScriptCreated @event) : base(@event)
  {
    ScriptId scriptId = new(@event.StreamId);
    WorldId = scriptId.WorldId.ResourceId;
    Id = scriptId.ResourceId;

    Name = @event.Name.Value;
  }

  private ScriptEntity() : base()
  {
  }

  public void Edit(ScriptEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(ScriptRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void Update(Script script)
  {
    base.Update(script);

    Name = script.Name.Value;
    Summary = script.Summary?.Value;
    Content = script.Content?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
