using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class TierConverter : JsonConverter<Tier>
{
  public override Tier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Tier(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Tier tier, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(tier.Value);
  }
}
