namespace SkillCraft.Api.Infrastructure.Entities;

public interface IEntitySerializer
{
  T? Deserialize<T>(string json);
  string? Serialize<T>(T entity);
}

internal class EntitySerializer : IEntitySerializer
{
  private static EntitySerializer? _instance = null;
  public static IEntitySerializer Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private readonly JsonSerializerOptions _serializerOptions = new();

  private EntitySerializer()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json);
  public string? Serialize<T>(T entity) => JsonSerializer.Serialize(entity, _serializerOptions);
}
