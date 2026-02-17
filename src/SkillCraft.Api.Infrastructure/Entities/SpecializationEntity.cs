using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Core.Specializations.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class SpecializationEntity : AggregateEntity, IWorldScoped
{
  public int SpecializationId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public TalentEntity? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }
  public Guid? RequiredTalentUid { get; private set; }

  public string? OtherRequirements { get; private set; }

  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

  public SpecializationEntity(WorldEntity world, SpecializationCreated @event) : base(@event)
  {
    Id = new SpecializationId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Tier = @event.Tier.Value;

    Name = @event.Name.Value;
  }

  private SpecializationEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (RequiredTalent is not null)
    {
      actorIds.AddRange(RequiredTalent.GetActorIds());
    }
    return actorIds;
  }

  public void Update(TalentEntity? requiredTalent, SpecializationUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Summary is not null)
    {
      Summary = @event.Summary.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Requirements is not null)
    {
      RequiredTalent = requiredTalent;
      RequiredTalentId = requiredTalent?.TalentId;
      RequiredTalentUid = requiredTalent?.Id;
      SetOtherRequirements(@event.Requirements.Other);
    }
  }

  public IReadOnlyCollection<string> GetOtherRequirements()
  {
    return (OtherRequirements is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(OtherRequirements)) ?? [];
  }
  private void SetOtherRequirements(IEnumerable<string> otherRequirements)
  {
    OtherRequirements = otherRequirements.Any() ? JsonSerializer.Serialize(otherRequirements) : null;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
