namespace SkillCraft.Api.Contracts.Specializations;

public record UpdateSpecializationPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  // TODO(fpion): Requirements { Talent, Other }
  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }
}
