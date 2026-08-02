namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageLanguageEntity
{
  public int LineageId { get; private set; }
  public int LanguageId { get; private set; }

  private LineageLanguageEntity()
  {
  }

  public override bool Equals(object? obj) => obj is LineageLanguageEntity entity && entity.LineageId == LineageId && entity.LanguageId == LanguageId;
  public override int GetHashCode() => HashCode.Combine(LineageId, LanguageId);
  public override string ToString() => $"{base.ToString()} (LineageId={LineageId}, LanguageId={LanguageId})";
}
