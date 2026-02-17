using SkillCraft.Api.Core.Specializations;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class SpecializationIdConverter : JsonConverter<SpecializationId>
{
  public override SpecializationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SpecializationId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SpecializationId specializationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(specializationId.Value);
  }
}
