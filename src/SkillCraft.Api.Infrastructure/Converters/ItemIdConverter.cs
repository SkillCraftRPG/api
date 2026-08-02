using SkillCraft.Api.Core.Items;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class ItemIdConverter : JsonConverter<ItemId>
{
  public override ItemId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ItemId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ItemId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
