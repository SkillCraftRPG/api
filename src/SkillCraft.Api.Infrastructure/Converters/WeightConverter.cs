using SkillCraft.Api.Core.Items;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class WeightConverter : JsonConverter<Weight>
{
  public override Weight? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetDouble(out double value) ? new Weight(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Weight weight, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(weight.Value);
  }
}
