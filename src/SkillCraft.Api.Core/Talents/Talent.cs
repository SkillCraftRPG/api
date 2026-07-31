using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class Talent : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Talent";

  public int TalentId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public bool AllowMultiplePurchases { get; private set; }
  public Skill? Skill { get; private set; }

  public Talent? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<Talent> RequiringTalents { get; private set; } = [];

  public Talent(World world, int tier, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Tier = tier;

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Talent()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    List<Guid> userIds = [CreatedBy, UpdatedBy];
    if (RequiredTalent is not null)
    {
      userIds.AddRange(RequiredTalent.GetUserIds());
    }
    return userIds.AsReadOnly();
  }

  public void SetAllowMultiplePurchases(bool allowMultiplePurchases)
  {
    if (allowMultiplePurchases && Skill.HasValue)
    {
      throw new InvalidTalentSkillException(this, Skill.Value);
    }

    AllowMultiplePurchases = allowMultiplePurchases;
  }

  public void SetSkill(Skill? skill)
  {
    if (AllowMultiplePurchases && skill.HasValue)
    {
      throw new InvalidTalentSkillException(this, skill.Value);
    }

    Skill = skill;
  }

  public void SetRequiredTalent(Talent? requiredTalent)
  {
    if (requiredTalent is not null && requiredTalent.Tier > Tier)
    {
      throw new InvalidRequiredTalentException(this, requiredTalent);
    }

    RequiredTalent = requiredTalent;
    RequiredTalentId = requiredTalent?.TalentId;
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Talent talent && talent.TalentId == TalentId;
  public override int GetHashCode() => TalentId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (TalentId={TalentId})";
}
