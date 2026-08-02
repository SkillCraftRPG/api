using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class CustomizationIdConverter : JsonConverter<CustomizationId>
{
  public override CustomizationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new CustomizationId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, CustomizationId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
