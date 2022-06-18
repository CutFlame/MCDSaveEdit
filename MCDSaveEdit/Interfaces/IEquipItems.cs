using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;
#nullable enable

namespace MCDSaveEdit.Interfaces
{
    public interface IEquipItems
    {
        IReadProperty<bool> equipmentCanExist { get; }
        IReadWriteProperty<int?> level { get; }
        IReadProperty<IEnumerable<Item>> equippedItemList { get; }
        void addEquippedItem(Item item);
        void selectItem(Item? item);
        IReadProperty<int?> remainingEnchantmentPoints { get; }
        Item? getGearItem(EquipmentSlotEnum slot);
        Item? meleeGearItem();
        Item? armorGearItem();
        Item? rangedGearItem();
        Item? hotbarSlot1Item();
        Item? hotbarSlot2Item();
        Item? hotbarSlot3Item();
        IReadProperty<int?> characterPower { get; }
    }
}
