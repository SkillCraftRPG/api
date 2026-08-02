using SkillCraft.Api.Core.Spells;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class SpellIdConverter : JsonConverter<SpellId>
{
  public override SpellId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SpellId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SpellId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
