using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class LineageIdConverter : JsonConverter<LineageId>
{
  public override LineageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new LineageId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, LineageId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
