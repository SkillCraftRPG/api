namespace SkillCraft.Api.Core.Items.Models;

public record ItemChargesModel
{
  public int Maximum { get; set; }
  public DepletionBehavior DepletionBehavior { get; set; }
  public ItemModel? Replacement { get; set; }
}
