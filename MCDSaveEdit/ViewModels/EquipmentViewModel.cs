using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public abstract class EquipmentViewModel
    {
        protected readonly Property<ProfileSaveFile?> _profile;
        public IReadProperty<ProfileSaveFile?> profile { get { return _profile; } }

        protected readonly Property<Item?> _selectedItem = new Property<Item?>(null);
        public IReadProperty<Item?> selectedItem { get { return _selectedItem; } }

        protected readonly Property<Enchantment?> _selectedEnchantment = new Property<Enchantment?>(null);
        public IReadProperty<Enchantment?> selectedEnchantment { get { return _selectedEnchantment; } }

        public IReadProperty<IEnumerable<Item>> equippedItemList { get; protected set; }
        public IReadWriteProperty<int?> level { get; protected set; }
        public IReadProperty<int?> characterPower { get; protected set; }
        public IReadProperty<bool> equipmentCanExist { get; protected set; }
        protected readonly Property<int?> _remainingEnchantmentPoints = new Property<int?>(null);
        public IReadProperty<int?> remainingEnchantmentPoints { get { return _remainingEnchantmentPoints; } }

        public EquipmentViewModel(Property<ProfileSaveFile?> profile)
        {
            _profile = profile;

            updateEnchantmentPoints();
        }

        public void updateEnchantmentPoints()
        {
            _remainingEnchantmentPoints.setValue = this.profile.value?.remainingEnchantmentPoints();
        }


        public void selectItem(Item? item)
        {
            EventLogger.logEvent("selectItem", new Dictionary<string, object>() { { "item", item?.Type ?? "null" } });
            _selectedItem.value = item;
        }

        public abstract void saveItem(Item item);

        protected virtual void triggerSubscribersForItem(Item item)
        {
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
            if (enchantments.Count >= Constants.MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM) { return; }
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            enchantments.Add(new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID, Level = 0 });
            selectedItem.value!.Enchantments = enchantments.Take(Constants.MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM).ToArray();

            saveItem(selectedItem.value!);
        }
    }
}
