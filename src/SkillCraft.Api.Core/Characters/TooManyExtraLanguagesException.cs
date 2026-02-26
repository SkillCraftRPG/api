using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters;

public class TooManyExtraLanguagesException : DomainException
{
  private const string ErrorMessage = "The selected extra languages exceeded the limit.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IReadOnlyCollection<Guid> LanguageIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(LanguageIds)]!;
    private set => Data[nameof(LanguageIds)] = value;
  }
  public int Count
  {
    get => (int)Data[nameof(Count)]!;
    private set => Data[nameof(Count)] = value;
  }
  public int Extra
  {
    get => (int)Data[nameof(Extra)]!;
    private set => Data[nameof(Extra)] = value;
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
      error.Data[nameof(LanguageIds)] = LanguageIds;
      error.Data[nameof(Count)] = Count;
      error.Data[nameof(Extra)] = Extra;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TooManyExtraLanguagesException(Lineage lineage, IEnumerable<LanguageId> languageIds, string propertyName) : base(BuildMessage(lineage, languageIds, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LanguageIds = GetExtraLanguageIds(lineage, languageIds);
    Count = LanguageIds.Count;
    Extra = lineage.Languages.Extra;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, IEnumerable<LanguageId> languageIds, string propertyName)
  {
    IReadOnlyCollection<Guid> extraLanguageIds = GetExtraLanguageIds(lineage, languageIds);

    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(lineage.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(Count)).Append(": ").Append(extraLanguageIds.Count).AppendLine();
    message.Append(nameof(Extra)).Append(": ").Append(lineage.Languages.Extra).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(LanguageIds)).AppendLine(":");
    foreach (Guid languageId in extraLanguageIds)
    {
      message.Append(" - ").Append(languageId).AppendLine();
    }
    return message.ToString();
  }

  private static IReadOnlyCollection<Guid> GetExtraLanguageIds(Lineage lineage, IEnumerable<LanguageId> languageIds) => languageIds
    .Except(lineage.Languages.Ids)
    .Select(languageId => languageId.EntityId)
    .Distinct()
    .ToList()
    .AsReadOnly();
}
