namespace SkillCraft.Api.Core;

public interface IAuditable
{
  Guid CreatedBy { get; }
  DateTime CreatedOn { get; }
  Guid UpdatedBy { get; }
  DateTime UpdatedOn { get; }
}
