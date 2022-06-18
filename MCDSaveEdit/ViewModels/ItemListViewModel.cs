using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public abstract class ItemListViewModel: EquipmentViewModel
    {
        protected static IEnumerable<Item> applyFilterToItems(IEnumerable<Item> items, ItemFilterEnum filter)
        {
            switch (filter)
            {
                case ItemFilterEnum.All: return items;
                case ItemFilterEnum.MeleeWeapons: return items.Where(x => x.isMeleeWeapon());
                case ItemFilterEnum.RangedWeapons: return items.Where(x => x.isRangedWeapon());
                case ItemFilterEnum.Armor: return items.Where(x => x.isArmor());
                case ItemFilterEnum.Artifacts: return items.Where(x => x.isArtifact());
                case ItemFilterEnum.Enchanted: return items.Where(x => x.enchantmentPoints() > 0);
            }
            throw new NotImplementedException();
        }
        protected static Item? equipmentSlot(IEnumerable<Item> itemList, EquipmentSlotEnum equipmentSlot)
        {
            var equipmentSlotString = equipmentSlot.ToString();
            return itemList.FirstOrDefault(x => x.EquipmentSlot == equipmentSlotString);
        }
        protected static int? computeCharacterPower(IEnumerable<Item> items)
        {
            var melee = equipmentSlot(items, EquipmentSlotEnum.MeleeGear)?.Power ?? 0;
            var armor = equipmentSlot(items, EquipmentSlotEnum.ArmorGear)?.Power ?? 0;
            var ranged = equipmentSlot(items, EquipmentSlotEnum.RangedGear)?.Power ?? 0;
            var slot1 = equipmentSlot(items, EquipmentSlotEnum.HotbarSlot1)?.Power ?? 0;
            var slot2 = equipmentSlot(items, EquipmentSlotEnum.HotbarSlot2)?.Power ?? 0;
            var slot3 = equipmentSlot(items, EquipmentSlotEnum.HotbarSlot3)?.Power ?? 0;
            var characterPower = GameCalculator.characterPowerFromEquippedItemPowers(melee, armor, ranged, slot1, slot2, slot3);
            var chacarterDisplayPower = GameCalculator.levelFromPower(characterPower);
            return chacarterDisplayPower;
        }

        public ItemListViewModel(Property<ProfileSaveFile?> profile) : base(profile)
        {
            equipmentCanExist = _profile.map<ProfileSaveFile?, bool>(p => p != null);
            level = _profile.map<ProfileSaveFile?, int?>(
                p => p?.level() ?? Constants.MINIMUM_CHARACTER_LEVEL,
                setXpFromLevel);
            filteredItemList = _filter.map(getFilteredItems);
            equippedItemList = _profile.map<ProfileSaveFile?, IEnumerable<Item>>(p => p?.Items?.Where(x => x.EquipmentSlot != null) ?? new Item[0]);
            characterPower = equippedItemList.map(computeCharacterPower);

            _profile.subscribe(p => {
                //_remainingEnchantmentPoints.setValue = p?.remainingEnchantmentPoints(); //Not Needed
                this.filter.setValue = ItemFilterEnum.All;
                _selectedItem.value = null;
            });
            level.subscribe(_ => this.updateEnchantmentPoints());
            equippedItemList.subscribe(_ => this.updateEnchantmentPoints());
            selectedItem.subscribe(_ => this.updateEnchantmentPoints());
        }

        private void setXpFromLevel(ProfileSaveFile? p, int? value)
        {
            if (p == null || !value.HasValue) { return; }
            p!.Xp = GameCalculator.experienceForLevel(value.Value);
        }
        protected abstract IEnumerable<Item> items { get; set; }

        protected IEnumerable<Item> getFilteredItems(ItemFilterEnum filter)
        {
            var items = this.items.Where(x => x.EquipmentSlot == null) ?? new Item[0];
            return applyFilterToItems(items, filter).OrderBy(x => x.InventoryIndex!.Value);
        }


        protected readonly Property<ItemFilterEnum> _filter = new Property<ItemFilterEnum>(ItemFilterEnum.All);
        public IReadWriteProperty<ItemFilterEnum> filter { get { return _filter; } }

        public IReadProperty<IEnumerable<Item>> filteredItemList { get; protected set; }

        public void addItemToList(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = addingItemToCollection(this.items, item);
            this.items = inventory;

            triggerSubscribersForItem(item);
        }

        public void removeItem(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = this.items.removing(item);
            this.items = inventory;

            triggerSubscribersForItem(item);
        }

        public override void saveItem(Item item)
        {
            if (item == null || profile.value == null || selectedItem.value == null) { return; }
            var inventory = this.items.replacing(selectedItem.value!, item);
            this.items = inventory;

            triggerSubscribersForItem(item);
        }

        protected IEnumerable<Item> addingItemToCollection(IEnumerable<Item> collection, Item item)
        {
            var index = getMaxIndex(collection) + 1;
            item.InventoryIndex = index;
            return collection.Append(item);
        }

        private long getFirstAvailableIndex(IEnumerable<Item> collection)
        {
            var inventoryIndexes = new HashSet<long>(collection.Select(i => i.InventoryIndex ?? 0));
            long index = 0;
            while (inventoryIndexes.Contains(index))
            {
                index++;
            }
            return index;
        }

        private long getMaxIndex(IEnumerable<Item> collection)
        {
            var list = collection.ToList();
            var maxIndex = list.Count > 0 ? list.Select(i => i.InventoryIndex ?? 0).Max() : -1;
            return maxIndex;
        }
    }
}
