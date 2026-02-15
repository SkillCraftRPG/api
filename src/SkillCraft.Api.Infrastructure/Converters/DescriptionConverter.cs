using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class DescriptionConverter : JsonConverter<Description>
{
  public override Description? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Description.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Description description, JsonSerializerOptions options)
  {
    writer.WriteStringValue(description.Value);
  }
}
