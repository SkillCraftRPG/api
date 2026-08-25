namespace SkillCraft.Api.Core.Characters;

public record CharacterStatistics
{
  public CharacterStatistic Dodge { get; }
  public CharacterStatistic Initiative { get; }
  public CharacterStatistic Learning { get; }
  public CharacterStatistic Load { get; }
  public CharacterStatistic Power { get; }
  public CharacterStatistic Precision { get; }
  public CharacterStatistic Stamina { get; }
  public CharacterStatistic Stratagem { get; }
  public CharacterStatistic Strength { get; }
  public CharacterStatistic Vitality { get; }

  public CharacterStatistics(CharacterAttributes attributes, int level = 0, IEnumerable<CharacterModifier>? modifiers = null)
  {
    Dictionary<Statistic, int> statistics = new(capacity: 10);
    if (modifiers is not null)
    {
      foreach (CharacterModifier modifier in modifiers)
      {
        if (modifier.Kind == CharacterModifierKind.Statistic)
        {
          Statistic statistic = Enum.Parse<Statistic>(modifier.Target);
          statistics[statistic] = statistics.GetValueOrDefault(statistic) + modifier.Value;
        }
      }
    }

    Dodge = new CharacterStatistic(10 + attributes.Dexterity.Total, statistics.GetValueOrDefault(Statistic.Dodge));
    Initiative = new CharacterStatistic(2 * attributes.Senses.Total, statistics.GetValueOrDefault(Statistic.Initiative));
    Load = new CharacterStatistic(10 * (5 + attributes.Vigor.Total), statistics.GetValueOrDefault(Statistic.Load));

    Power = new CharacterStatistic(5 + (2 * attributes.Senses.Total), statistics.GetValueOrDefault(Statistic.Power));
    Precision = new CharacterStatistic(5 + (2 * attributes.Dexterity.Total), statistics.GetValueOrDefault(Statistic.Precision));
    Stratagem = new CharacterStatistic(5 + (2 * attributes.Intellect.Total), statistics.GetValueOrDefault(Statistic.Stratagem));
    Strength = new CharacterStatistic(5 + (2 * attributes.Vigor.Total), statistics.GetValueOrDefault(Statistic.Strength));

    int learning = (int)Math.Max(
      5 + attributes.Intellect.Total + (level / 5.0 * (2 + attributes.Intellect.Total)),
      5 + (level / 5.0));
    Learning = new CharacterStatistic(learning, statistics.GetValueOrDefault(Statistic.Learning));

    int constitution = (int)((25 + level) * (5 + attributes.Health.Total) / 5.0);
    Stamina = new CharacterStatistic(constitution, statistics.GetValueOrDefault(Statistic.Stamina));
    Vitality = new CharacterStatistic(constitution, statistics.GetValueOrDefault(Statistic.Vitality));
  }
}
