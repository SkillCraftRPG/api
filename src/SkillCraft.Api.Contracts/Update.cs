namespace SkillCraft.Api.Contracts;

public record Update<T>
{
  public T? Value { get; set; }

  public Update(T? value = default)
  {
    Value = value;
  }
}
