using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public class Resource
{
  private const char Separator = '|';
  private const char ResourceSeparator = ':';

  private readonly string _value;

  public WorldId? WorldId { get; }
  public string Kind { get; }
  public Guid Id { get; }

  public Resource(string kind, Guid id, WorldId? worldId = null)
  {
    if (string.IsNullOrWhiteSpace(kind))
    {
      throw new ArgumentException("The kind is required.", nameof(kind));
    }

    WorldId = worldId;
    Kind = kind.Trim();
    Id = id;

    string resource = string.Join(ResourceSeparator, kind, Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64());
    _value = WorldId.HasValue ? string.Join(Separator, WorldId.Value, resource) : resource;
  }

  public static Resource Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{value}' is not a valid resource identifier.", nameof(value));
    }

    WorldId? worldId = values.Length == 2 ? new(values.First()) : null;

    string[] parts = values.Last().Split(ResourceSeparator);
    if (parts.Length != 2)
    {
      throw new ArgumentException($"The value '{parts.Last()}' is not a valid resource.", nameof(value));
    }

    string kind = parts.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The resource kind '{kind}' was not expected ({expectedKind}).");
    }
    Guid id = new(Convert.FromBase64String(parts.Last().FromUriSafeBase64()));

    return new Resource(kind, id, worldId);
  }

  public override bool Equals(object? obj) => obj is Resource resource && resource._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}
