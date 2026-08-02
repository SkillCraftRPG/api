using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CustomizationEntity : AggregateEntity
{
  public int CustomizationId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public CustomizationKind Kind { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public CustomizationEntity(Customization customization) : base(customization)
  {
    WorldId = customization.WorldId.ResourceId;
    Id = customization.ResourceId;

    Kind = customization.Kind;

    Update(customization);
  }

  public CustomizationEntity(CustomizationCreated @event) : base(@event)
  {
    CustomizationId customizationId = new(@event.StreamId);
    WorldId = customizationId.WorldId.ResourceId;
    Id = customizationId.ResourceId;

    Kind = @event.Kind;

    Name = @event.Name.Value;
  }

  private CustomizationEntity() : base()
  {
  }

  public void Edit(CustomizationEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(CustomizationRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void Update(Customization customization)
  {
    base.Update(customization);

    Name = customization.Name.Value;
    Summary = customization.Summary?.Value;
    Content = customization.Content?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
