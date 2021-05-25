using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
#nullable enable

namespace MCDSaveEdit
{
    public class ProfileViewModel
    {
        public static readonly Dictionary<string, string> supportedFileTypesDict = new Dictionary<string, string>
        {
            {"*" + Constants.ENCRYPTED_FILE_EXTENSION, R.ENCRYPTED_CHARACTER_SAVE_FILES },
            {"*" + Constants.DECRYPTED_FILE_EXTENSION, R.DECRYPTED_CHARACTER_SAVE_FILES },
            {"*.*", R.ALL_FILES },
        };


        public string? filePath { get; set; }

        private Property<ProfileSaveFile?> _profile = new Property<ProfileSaveFile?>(null);
        public IReadWriteProperty<ProfileSaveFile?> profile { get { return _profile; } }

        private Property<Item?> _selectedItem = new Property<Item?>(null);
        public IReadProperty<Item?> selectedItem { get { return _selectedItem; } }

        private Property<Enchantment?> _selectedEnchantment = new Property<Enchantment?>(null);
        public IReadProperty<Enchantment?> selectedEnchantment { get { return _selectedEnchantment; } }

        private Property<ItemFilterEnum> _filter = new Property<ItemFilterEnum>(ItemFilterEnum.All);
        public IReadWriteProperty<ItemFilterEnum> filter { get { return _filter; } }

        public IReadProperty<IEnumerable<Item>> filteredItemList;
        public IReadProperty<IEnumerable<Item>> equippedItemList;
        public IReadWriteProperty<int?> level;
        public IReadWriteProperty<ulong?> emeralds;
        public IReadWriteProperty<ulong?> gold;

        public ProfileViewModel()
        {
            level = _profile.map<ProfileSaveFile?, int?>(
                p => p?.level() ?? Constants.MINIMUM_CHARACTER_LEVEL,
                (p, value) => {
                    if (p == null || !value.HasValue) { return; }
                    p!.Xp = GameCalculator.experienceForLevel(value.Value);
                });

            emeralds = _profile.map<ProfileSaveFile?, ulong?>(
               p => p?.Currency.FirstOrDefault(c => c.Type == Constants.EMERALD_CURRENCY_NAME)?.Count,
               (p, value) => {
                   if (p == null || value == null) { return; }
                   Currency currency = p!.Currency.FirstOrDefault(c => c.Type == Constants.EMERALD_CURRENCY_NAME) ?? new Currency() { Type = Constants.EMERALD_CURRENCY_NAME };
                   currency.Count = value.Value;
                   p!.Currency = (new[] { currency }).Concat(p!.Currency.Where(c => c.Type != Constants.EMERALD_CURRENCY_NAME)).OrderBy(c => c.Type).ToArray();
               });

            gold = _profile.map<ProfileSaveFile?, ulong?>(
               p => p?.Currency.FirstOrDefault(c => c.Type == Constants.GOLD_CURRENCY_NAME)?.Count,
               (p, value) => {
                   if (p == null || value == null) { return; }
                   Currency currency = p!.Currency.FirstOrDefault(c => c.Type == Constants.GOLD_CURRENCY_NAME) ?? new Currency() { Type = Constants.GOLD_CURRENCY_NAME };
                   currency.Count = value.Value;
                   p!.Currency = (new[] { currency }).Concat(p!.Currency.Where(c => c.Type != Constants.GOLD_CURRENCY_NAME)).OrderBy(c => c.Type).ToArray();
               });

            filteredItemList = _filter.map<ItemFilterEnum, IEnumerable<Item>>(
                f => {
                    var items = this.profile.value?.unequippedItems() ?? new Item[0];
                    return applyFilter(f, items).OrderBy(x => x.InventoryIndex!.Value).ToArray();
                });

            equippedItemList = _profile.map<ProfileSaveFile?, IEnumerable<Item>>(p => p?.equippedItems() ?? new Item[0]);

            profile.subscribe(p => { this.filter.setValue = ItemFilterEnum.All; _selectedItem.value = null; });
        }

        private static IEnumerable<Item> applyFilter(ItemFilterEnum filter, IEnumerable<Item> items)
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

        public void selectItem(Item? item)
        {
            EventLogger.logEvent("selectItem", new Dictionary<string, object>() { { "item", item?.Type ?? "null" } });
            _selectedItem.value = item;
        }

        public void addEquippedItem(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = profile.value!.Items.ToList();
            inventory.Add(item);
            profile.value!.Items = inventory.ToArray();

            triggerSubscribersForItem(item);
        }


        public void addItemToInventory(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = profile.value!.Items.ToList();
            var maxIndex = inventory.Count > 0 ? inventory.Select(i => i.InventoryIndex ?? 0).Max() : -1;
            item.InventoryIndex = maxIndex + 1;
            inventory.Add(item);
            profile.value!.Items = inventory.ToArray();

            triggerSubscribersForItem(item);
        }

        public void removeItem(Item item)
        {
            if (item == null || profile.value == null) { return; }
            var inventory = profile.value!.Items.ToList();
            var index = inventory.IndexOf(item!);

            if(index < 0 || index >= inventory.Count)
            {
                EventLogger.logError($"Could not get valid index for item: {item}");
                return;
            }
            inventory.RemoveAt(index);
            profile.value!.Items = inventory.ToArray();

            triggerSubscribersForItem(item);
        }

        public void saveItem(Item item)
        {
            if (item == null || profile.value == null || selectedItem.value == null) { return; }
            var inventory = profile.value!.Items.ToList();
            var index = inventory.IndexOf(selectedItem.value!);

            if (index < 0 || index >= inventory.Count)
            {
                EventLogger.logError($"Could not get valid index for item: {item}");
                return;
            }
            inventory.RemoveAt(index);
            inventory.Insert(index, item);
            profile.value!.Items = inventory.ToArray();

            triggerSubscribersForItem(item);
        }

        private void triggerSubscribersForItem(Item item)
        {
            if (item.EquipmentSlot != null)
            {
                ((MappedProperty<ProfileSaveFile?, IEnumerable<Item>>)this.equippedItemList).value = this.equippedItemList.value;
            }
            if (item.InventoryIndex != null)
            {
                ((MappedProperty<ItemFilterEnum, IEnumerable<Item>>)this.filteredItemList).value = this.filteredItemList.value;
            }

            _selectedItem.value = _selectedItem.value;
        }

        public void selectEnchantment(Enchantment? enchantment)
        {
            EventLogger.logEvent("selectEnchantment", new Dictionary<string, object>() { { "enchantment", enchantment?.Id ?? "null" } });
            _selectedEnchantment.value = enchantment;
        }

        public void saveEnchantment(Enchantment enchantment)
        {
            if (enchantment == null || profile.value == null || selectedItem.value == null || selectedEnchantment.value == null) { return; }
            var enchantments = selectedItem.value!.Enchantments.ToList();
            var index = enchantments.IndexOf(selectedEnchantment.value!);

            enchantments.RemoveAt(index);
            enchantments.Insert(index, enchantment);
            selectedItem.value!.Enchantments = enchantments.ToArray();

            saveItem(selectedItem.value!);
        }

        public void addEnchantmentSlot(object sender)
        {
            if (profile.value == null || selectedItem.value == null) { return; }
            var enchantments = selectedItem.value!.Enchantments?.ToList() ?? new List<Enchantment>();
            if(enchantments.Count >= Constants.MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM) { return; }
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            selectedItem.value!.Enchantments = enchantments.Take(Constants.MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM).ToArray();

            saveItem(selectedItem.value!);
        }
    }
}
