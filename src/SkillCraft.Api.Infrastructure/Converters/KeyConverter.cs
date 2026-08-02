using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class KeyConverter : JsonConverter<Key>
{
  public override Key? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Key.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Key key, JsonSerializerOptions options)
  {
    writer.WriteStringValue(key.Value);
  }
}
