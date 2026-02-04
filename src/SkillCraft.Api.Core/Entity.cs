using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public record Entity(string Kind, Guid Id, WorldId? WorldId = null)
{
  private const char Separator = '|';
  private const char EntitySeparator = ':';

  public static Entity Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{value}' is not a valid entity.", nameof(value));
    }

    string[] entity = values.Last().Split(EntitySeparator);
    if (entity.Length != 2)
    {
      throw new ArgumentException($"The entity '{values.Last()}' is not valid.", nameof(value));
    }

    string kind = entity.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The entity kind '{expectedKind}' was expected, but '{kind}' was received.", nameof(value));
    }

    Guid id = new(Convert.FromBase64String(entity.Last().FromUriSafeBase64()));
    WorldId? worldId = values.Length < 2 ? null : new(values.First());

    return new(kind, id, worldId);
  }

  public override string ToString()
  {
    string entity = string.Join(EntitySeparator, Kind, Convert.ToBase64String(Id.ToByteArray()).ToUriSafeBase64());
    return WorldId.HasValue ? string.Join(Separator, WorldId.Value, entity) : entity;
  }
}
