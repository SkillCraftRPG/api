namespace SkillCraft.Api.Core.Characters;

public interface IExperienceTable
{
  int GetLevel(int experience);
  int GetThreshold(int level);
}

public class ExperienceTable : IExperienceTable
{
  private static ExperienceTable? _instance = null;
  public static IExperienceTable Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private readonly int[] _thresholds = new int[Character.MaximumLevel];

  private ExperienceTable()
  {
    for (int level = 1; level <= Character.MaximumLevel; level++)
    {
      _thresholds[level - 1] = (int)Math.Pow(level, 2) * 100;
    }
  }

  public int GetLevel(int experience)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(experience);

    for (int level = 0; level < Character.MaximumLevel; level++)
    {
      if (experience < _thresholds[level])
      {
        return level;
      }
    }

    return Character.MaximumLevel;
  }

  public int GetThreshold(int level)
  {
    if (level < 0 || level > Character.MaximumLevel)
    {
      throw new ArgumentOutOfRangeException(nameof(level));
    }
    else if (level == 0)
    {
      return 0;
    }
    return _thresholds[level - 1];
  }
}
