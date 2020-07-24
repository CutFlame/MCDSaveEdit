using DungeonTools.Save.File;
using DungeonTools.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit
{
    public class ProfileViewModel
    {
        public static readonly Dictionary<string, string> supportedFileTypesDict = new Dictionary<string, string>
        {
            {"*.dat", R.ENCRYPTED_CHARACTER_SAVE_FILES },
            {"*.json", R.DECRYPTED_CHARACTER_SAVE_FILES },
            {"*.*", R.ALL_FILES },
        };


        public Action<string>? showError;

        public string? filePath { get; private set; }
        
        private Property<ProfileSaveFile?> _profile = new Property<ProfileSaveFile?>(null);
        public IReadProperty<ProfileSaveFile?> profile { get { return _profile; } }
        
        private Property<Item?> _selectedItem = new Property<Item?>(null);
        public IReadProperty<Item?> selectedItem { get { return _selectedItem; } }

        private Property<Enchantment?> _selectedEnchantment = new Property<Enchantment?>(null);
        public IReadProperty<Enchantment?> selectedEnchantment { get { return _selectedEnchantment; } }

        private Property<ItemFilterEnum> _filter = new Property<ItemFilterEnum>(ItemFilterEnum.All);
        public IWriteProperty<ItemFilterEnum> filter { get { return _filter; } }

        public IReadProperty<IEnumerable<Item>> filteredItemList;
        public IReadProperty<IEnumerable<Item>> equippedItemList;
        public IReadWriteProperty<int?> level;
        public IReadWriteProperty<uint?> emeralds;

        public ProfileViewModel()
        {
            level = new MappedProperty<ProfileSaveFile?, int?>(_profile,
                p => p?.level() ?? Constants.MINIMUM_CHARACTER_LEVEL,
                (p, value) =>
                {
                    if (p == null || !value.HasValue) { return; }
                    p!.Experience = GameCalculator.experienceForLevel(value.Value);
                });

            emeralds = new MappedProperty<ProfileSaveFile?, uint?>(_profile,
                p => p?.Currencies.FirstOrDefault()?.Amount,
                (p, value) =>
                {
                    if (p == null || value == null) { return; }
                    Currency currency = p!.Currencies.FirstOrDefault() ?? new Currency() { Name = "Emerald" };
                    currency.Amount = value.Value;
                    p!.Currencies = (new[] { currency }).Concat(p!.Currencies.Skip(1));
                });

            filteredItemList = new MappedProperty<ItemFilterEnum, IEnumerable<Item>>(_filter,
                f => {
                    var items = this.profile.value?.unequippedItems() ?? new Item[0];
                    return applyFilter(f, items).OrderBy(x => x.InventorySlot!).ToArray();
                });

            equippedItemList = new MappedProperty<ProfileSaveFile?, IEnumerable<Item>>(_profile, p => p?.equippedItems() ?? new Item[0]);

            profile.subscribe(p => this.filter.setValue = ItemFilterEnum.All);
        }

        #region Open File

        public async Task handleFileOpenAsync(string? filePath)
        {
            if (filePath == null) { return; }
            Console.WriteLine("Reading file: {0}", filePath!);
            if (Path.GetExtension(filePath!) == ".json")
            {
                await handleJsonFileOpen(filePath!);
            }
            else
            {
                await handleDatFileOpen(filePath!);
            }
        }

        private async Task handleJsonFileOpen(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this.filePath = filePath;
            await tryParseFileStreamAsync(stream);
        }

        private async Task handleDatFileOpen(string filePath)
        {
            var file = new FileInfo(filePath);
            using FileStream inputStream = file.OpenRead();
            bool encrypted = SaveFileHandler.IsFileEncrypted(inputStream);
            if (!encrypted)
            {
                await Console.Out.WriteLineAsync($"[  ERROR  ] The file \"{file.Name}\" was in an unexpected format.");
                showError?.Invoke(R.formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(file.Name));
                return;
            }
            using Stream? processed = await FileProcessHelper.Decrypt(inputStream);
            if (processed == null)
            {
                await Console.Out.WriteLineAsync($"[  ERROR  ] Content of file \"{file.Name}\" could not be converted to a supported format.");
                showError?.Invoke(R.formatFILE_DECRYPT_ERROR_MESSAGE(file.Name));
                return;
            }
            this.filePath = filePath;
            await tryParseFileStreamAsync(processed!);
        }

        private async Task tryParseFileStreamAsync(Stream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                var profile = await ProfileParser.Read(stream);
                _profile.value = profile;
                _selectedItem.value = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                showError?.Invoke(R.FAILED_TO_PARSE_FILE_ERROR_MESSAGE);
            }
        }

        #endregion

        #region Save File

        public async Task handleFileSaveAsync(string? filePath)
        {
            if(filePath == null) { return; }
            Console.WriteLine("Writing file: {0}", filePath!);
            if (Path.GetExtension(filePath!) == ".json")
            {
                await handleJsonFileSave(filePath!);
            }
            else
            {
                await handleDatFileSave(filePath!);
            }
        }

        private async Task handleJsonFileSave(string filePath)
        {
            using var stream = await ProfileParser.Write(profile.value);
            await writeStreamToFileAsync(stream, filePath);
        }

        private async Task handleDatFileSave(string filePath)
        {
            using var inputStream = await ProfileParser.Write(profile.value);
            inputStream.Seek(0, SeekOrigin.Begin);
            using Stream? processed = await FileProcessHelper.Encrypt(inputStream);
            if (processed == null)
            {
                await Console.Out.WriteLineAsync($"[  ERROR  ] Failed to encrypt the json data.");
                showError?.Invoke(R.FAILED_TO_ENCRYPT_ERROR_MESSAGE);
                return;
            }
            await writeStreamToFileAsync(processed, filePath);
        }

        private async Task writeStreamToFileAsync(Stream stream, string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
            }
            using var filestream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(filestream);
        }

        #endregion

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
            _selectedItem.value = item;
        }

        public void saveItem(Item item)
        {
            if (item == null || profile.value == null || selectedItem.value == null) { return; }
            var inventory = profile.value!.Inventory.ToList();
            var index = inventory.IndexOf(selectedItem.value!);

            inventory.RemoveAt(index);
            inventory.Insert(index, item);
            profile.value!.Inventory = inventory;

            if (item.EquipmentSlot != null)
            {
                ((MappedProperty<ProfileSaveFile?, IEnumerable<Item>>)this.equippedItemList).value = this.equippedItemList.value;
            }
            if (item.InventorySlot != null)
            {
                ((MappedProperty<ItemFilterEnum, IEnumerable<Item>>)this.filteredItemList).value = this.filteredItemList.value;
            }

            _selectedItem.value = _selectedItem.value;
        }

        public void selectEnchantment(Enchantment? enchantment)
        {
            _selectedEnchantment.value = enchantment;
        }

        public void saveEnchantment(Enchantment enchantment)
        {
            if (enchantment == null || profile.value == null || selectedItem.value == null || selectedEnchantment.value == null) { return; }
            var enchantments = selectedItem.value!.Enchantments.ToList();
            var index = enchantments.IndexOf(selectedEnchantment.value!);

            enchantments.RemoveAt(index);
            enchantments.Insert(index, enchantment);
            selectedItem.value!.Enchantments = enchantments;

            saveItem(selectedItem.value!);
        }

    }
}
