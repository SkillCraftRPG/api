using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure;

public interface IRecordSerializer
{
  string Serialize(ChangeEvent @event);
}

internal class RecordSerializer : IRecordSerializer
{
  private static RecordSerializer? _instance = null;
  public static IRecordSerializer Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private readonly JsonSerializerOptions _serializerOptions = new();

  public RecordSerializer()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public string Serialize(ChangeEvent @event) => JsonSerializer.Serialize(@event, _serializerOptions);
}
