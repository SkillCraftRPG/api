using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public class WorldMismatchException : ArgumentException
{
  private const string ErrorMessage = "The world identifiers are not the same.";

  public Guid Expected
  {
    get => (Guid)Data[nameof(Expected)]!;
    private set => Data[nameof(Expected)] = value;
  }
  public Guid Actual
  {
    get => (Guid)Data[nameof(Actual)]!;
    private set => Data[nameof(Actual)] = value;
  }

  public WorldMismatchException(WorldId expected, WorldId actual, string propertyName)
    : base(BuildMessage(expected, actual), propertyName)
  {
    Expected = expected.ResourceId;
    Actual = actual.ResourceId;
  }

  public static void ThrowIfMismatch(WorldId expected, WorldId actual, string propertyName)
  {
    if (expected != actual)
    {
      throw new WorldMismatchException(expected, actual, propertyName);
    }
  }

  private static string BuildMessage(WorldId expected, WorldId actual) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Expected), expected.ResourceId)
    .AddData(nameof(Actual), actual.ResourceId)
    .Build();
}
