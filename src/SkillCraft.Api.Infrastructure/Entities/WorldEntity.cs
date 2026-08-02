using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class WorldEntity : AggregateEntity
{
  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;

  public string Key { get; private set; } = string.Empty;
  public string? Name { get; private set; }
  public string? Content { get; private set; }

  public List<Caste> Castes { get; private set; } = [];
  public List<CustomizationEntity> Customizations { get; private set; } = [];
  public List<Education> Educations { get; private set; } = [];
  public List<Item> Items { get; private set; } = [];
  public List<Language> Languages { get; private set; } = [];
  public List<Lineage> Lineages { get; private set; } = [];
  public List<Script> Scripts { get; private set; } = [];
  public List<Spell> Spells { get; private set; } = [];
  public List<TalentEntity> Talents { get; private set; } = [];

  public WorldEntity(World world) : base(world)
  {
    Id = world.ResourceId;

    OwnerId = world.OwnerId.Value;

    Update(world);
  }

  public WorldEntity(WorldCreated @event) : base(@event)
  {
    Id = new WorldId(@event.StreamId).ResourceId;

    OwnerId = @event.OwnerId.Value;

    Key = @event.Key.Value;
  }

  private WorldEntity() : base()
  {
  }

  public void Edit(WorldEdited @event)
  {
    base.Update(@event);

    Content = @event.Content?.Value;
  }

  public void Rename(WorldRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name?.Value;
  }

  public void SetKey(WorldKeyChanged @event)
  {
    base.Update(@event);

    Key = @event.Key.Value;
  }

  public void Update(World world)
  {
    base.Update(world);

    Key = world.Key.Value;
    Name = world.Name?.Value;
    Content = world.Content?.Value;
  }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
