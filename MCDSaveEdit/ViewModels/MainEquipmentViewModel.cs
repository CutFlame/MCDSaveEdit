using MCDSaveEdit.Data;
using MCDSaveEdit.Interfaces;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public class MainEquipmentViewModel : ItemListViewModel, IEquipItems, IListItems
    {
        public MainEquipmentViewModel(Property<ProfileSaveFile?> profile) : base(profile)
        {
        }

        protected override IEnumerable<Item> items {
            get {
                return profile.value?.Items ?? new Item[0];
            }
            set {
                if (profile.value == null) return;
                profile.value!.Items = value.ToArray();
            }
        }

        public void addEquippedItem(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = this.items;
            if (item.isGearItem())
            {
                var toRemove = item.EquipmentSlot;
                inventory = inventory.Where(i => i.EquipmentSlot != toRemove);
            }                
            this.items = inventory.adding(item);

            triggerSubscribersForItem(item);
        }

        protected override void triggerSubscribersForItem(Item item)
        {
            if (item.InventoryIndex != null)
            {
                ((MappedProperty<ItemFilterEnum, IEnumerable<Item>>)this.filteredItemList).value = this.filteredItemList.value;
            }
            if (item.EquipmentSlot != null)
            {
                ((MappedProperty<ProfileSaveFile?, IEnumerable<Item>>)this.equippedItemList).value = this.equippedItemList.value;
            }

            base.triggerSubscribersForItem(item);
        }

        public Item? getGearItem(EquipmentSlotEnum slot)
        {
            return equipmentSlot(equippedItemList.value, slot);
        }

        public Item? meleeGearItem()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.MeleeGear);
        }
        public Item? armorGearItem()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.ArmorGear);
        }
        public Item? rangedGearItem()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.RangedGear);
        }
        public Item? hotbarSlot1Item()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.HotbarSlot1);
        }
        public Item? hotbarSlot2Item()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.HotbarSlot2);
        }
        public Item? hotbarSlot3Item()
        {
            return equipmentSlot(equippedItemList.value, EquipmentSlotEnum.HotbarSlot3);
        }

    }
}
