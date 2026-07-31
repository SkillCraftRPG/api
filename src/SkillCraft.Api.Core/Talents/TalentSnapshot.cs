using SkillCraft.Api.Core.Talents.Events;

namespace SkillCraft.Api.Core.Talents;

public record TalentSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public bool AllowMultiplePurchases { get; }
  public Skill? Skill { get; }
  public Guid? RequiredTalentId { get; }

  public TalentSnapshot(Talent talent)
  {
    Name = talent.Name;
    Summary = talent.Summary;
    Content = talent.Content;

    AllowMultiplePurchases = talent.AllowMultiplePurchases;
    Skill = talent.Skill;
    RequiredTalentId = talent.RequiredTalent?.Id;
  }

  public TalentUpdated? Compare(Talent talent)
  {
    int changes = 0;
    TalentUpdated record = new(talent);

    if (Name != talent.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, talent.Name);
    }

    if (Summary != talent.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, talent.Summary);
    }

    if (Content != talent.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, talent.Content);
    }

    if (AllowMultiplePurchases != talent.AllowMultiplePurchases)
    {
      changes++;
      record.AllowMultiplePurchases = new Change<bool>(AllowMultiplePurchases, talent.AllowMultiplePurchases);
    }

    if (Skill != talent.Skill)
    {
      changes++;
      record.Skill = new Change<Skill?>(Skill, talent.Skill);
    }

    if (RequiredTalentId != talent.RequiredTalent?.Id)
    {
      changes++;
      record.RequiredTalentId = new Change<Guid?>(RequiredTalentId, talent.RequiredTalent?.Id);
    }

    return changes < 1 ? null : record;
  }
}
