using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class LanguageIdConverter : JsonConverter<LanguageId>
{
  public override LanguageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new LanguageId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, LanguageId languageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(languageId.Value);
  }
}
