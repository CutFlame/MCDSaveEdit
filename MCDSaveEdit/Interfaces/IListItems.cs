using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public interface IListItems
    {
        IReadProperty<ProfileSaveFile?> profile { get; }
        IReadWriteProperty<ItemFilterEnum> filter { get; }
        IReadProperty<IEnumerable<Item>> filteredItemList { get; }
        void selectItem(Item item);

        void addItemToList(Item item);
    }
}
