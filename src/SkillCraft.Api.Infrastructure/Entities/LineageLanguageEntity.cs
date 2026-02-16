namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageLanguageEntity
{
  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public Guid LineageUid { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int LanguageId { get; private set; }
  public Guid LanguageUid { get; private set; }

  public LineageLanguageEntity(LineageEntity lineage, LanguageEntity language)
  {
    Lineage = lineage;
    LineageId = lineage.LineageId;
    LanguageUid = lineage.Id;

    Language = language;
    LanguageId = language.LanguageId;
    LanguageUid = language.Id;
  }

  private LineageLanguageEntity()
  {
  }

  public override bool Equals(object? obj) => obj is LineageLanguageEntity entity && entity.LineageId == LineageId && entity.LanguageId == LanguageId;
  public override int GetHashCode() => HashCode.Combine(LineageId, LanguageId);
  public override string ToString() => $"{GetType()} (LineageId={LineageId}, LanguageId={LanguageId})";
}
