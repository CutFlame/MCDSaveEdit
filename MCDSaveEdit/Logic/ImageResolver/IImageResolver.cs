using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public interface IImageResolver
    {
        string? path { get; }
        IReadOnlyCollection<string> localizationOptions { get; }
        Dictionary<string, Dictionary<string, string>>? loadLanguageStrings(string langSpecifier);
        BitmapImage? imageSourceForItem(Item item);
        BitmapImage? imageSourceForItem(string itemType);
        BitmapImage? imageSourceForRarity(Rarity rarity);
        BitmapImage? imageSourceForEnchantment(Enchantment enchantment);
        BitmapImage? imageSourceForEnchantment(string enchantmentType);
        BitmapImage? imageSource(string path);
    }
}
