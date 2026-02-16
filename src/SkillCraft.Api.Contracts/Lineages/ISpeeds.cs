namespace SkillCraft.Api.Contracts.Lineages;

public interface ISpeeds
{
  int Walk { get; }
  int Climb { get; }
  int Swim { get; }
  int Fly { get; }
  bool Hover { get; }
  int Burrow { get; }
}
