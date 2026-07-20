using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class InvalidTalentSkillException : DomainException
{
  private const string ErrorMessage = "The specified talent cannot mutually allow multiple purchases and train a skill.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public Skill AttemptedSkill
  {
    get => (Skill)Data[nameof(AttemptedSkill)]!;
    private set => Data[nameof(AttemptedSkill)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(WorldId)] = WorldId;
      error.Data[nameof(TalentId)] = TalentId;
      error.Data[nameof(AttemptedSkill)] = AttemptedSkill;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidTalentSkillException(Talent talent)
    : base(BuildMessage(talent))
  {
    if (!talent.AllowMultiplePurchases)
    {
      throw new ArgumentException("The talent must allow multiple purchases.", nameof(talent));
    }
    if (!talent.Skill.HasValue)
    {
      throw new ArgumentException("The skill is required.", nameof(talent));
    }

    WorldId = talent.WorldId;
    TalentId = talent.Id;
    AttemptedSkill = talent.Skill.Value;
    PropertyName = nameof(Talent.Skill);
  }

  private static string BuildMessage(Talent talent) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId)
    .AddData(nameof(TalentId), talent.Id)
    .AddData(nameof(AttemptedSkill), talent.Skill)
    .AddData(nameof(PropertyName), nameof(Talent.Skill))
    .Build();
}
