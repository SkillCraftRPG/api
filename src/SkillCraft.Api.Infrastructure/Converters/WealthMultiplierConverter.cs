using SkillCraft.Api.Core.Educations;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class WealthMultiplierConverter : JsonConverter<WealthMultiplier>
{
  public override WealthMultiplier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new WealthMultiplier(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, WealthMultiplier wealthMultiplier, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(wealthMultiplier.Value);
  }
}
