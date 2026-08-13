using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items;

internal static class ItemHelper
{
  public static MagicItem? GetMagicItem(MagicItemModel? model)
  {
    if (model is null)
    {
      return null;
    }

    Attunement? attunement = model.Attunement is null ? null : new(model.Attunement);
    return new MagicItem(attunement);
  }
}
