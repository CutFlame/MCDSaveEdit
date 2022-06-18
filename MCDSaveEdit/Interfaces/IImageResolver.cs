using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.Interfaces
{
    public interface IImageResolver
    {
        string? path { get; }
        BitmapImage? imageSourceForItem(Item item);
        BitmapImage? imageSourceForItem(string itemType);
        BitmapImage? imageSourceForRarity(Rarity rarity);
        BitmapImage? imageSourceForEnchantment(Enchantment enchantment);
        BitmapImage? imageSourceForEnchantment(string enchantmentType);
        BitmapImage? imageSource(string path);
    }
}
