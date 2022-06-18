using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public class StorageChestEquipmentViewModel : ItemListViewModel, IListItems
    {
        public StorageChestEquipmentViewModel(Property<ProfileSaveFile?> profile) : base(profile)
        {
        }

        protected override IEnumerable<Item> items {
            get {
                return profile.value?.StorageChestItems ?? new Item[0];
            }
            set {
                if (profile.value == null) return;
                profile.value!.StorageChestItems = value.ToArray();
            }
        }

        protected override void triggerSubscribersForItem(Item item)
        {
            if (item.InventoryIndex != null)
            {
                ((MappedProperty<ItemFilterEnum, IEnumerable<Item>>)this.filteredItemList).value = this.filteredItemList.value;
            }

            base.triggerSubscribersForItem(item);
        }
    }
}
