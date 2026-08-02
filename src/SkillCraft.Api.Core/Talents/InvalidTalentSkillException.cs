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

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(WorldId)] = WorldId;
      error.Data[nameof(TalentId)] = TalentId;
      error.Data[nameof(AttemptedSkill)] = AttemptedSkill;
      return error;
    }
  }

  public InvalidTalentSkillException(Talent talent, Skill attemptedSkill)
    : base(BuildMessage(talent, attemptedSkill))
  {
    WorldId = talent.WorldId.ResourceId;
    TalentId = talent.ResourceId;
    AttemptedSkill = attemptedSkill;
  }

  private static string BuildMessage(Talent talent, Skill attemptedSkill) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId.ResourceId)
    .AddData(nameof(TalentId), talent.ResourceId)
    .AddData(nameof(AttemptedSkill), attemptedSkill)
    .Build();
}
