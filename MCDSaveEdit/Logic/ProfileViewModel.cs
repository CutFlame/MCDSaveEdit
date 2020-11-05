using DungeonTools.Save.File;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
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
            {"*" + Constants.ENCRYPTED_FILE_EXTENSION, R.ENCRYPTED_CHARACTER_SAVE_FILES },
            {"*" + Constants.DECRYPTED_FILE_EXTENSION, R.DECRYPTED_CHARACTER_SAVE_FILES },
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
        public IReadWriteProperty<ulong?> emeralds;

        public ProfileViewModel()
        {
            level = new MappedProperty<ProfileSaveFile?, int?>(_profile,
                p => p?.level() ?? Constants.MINIMUM_CHARACTER_LEVEL,
                (p, value) =>
                {
                    if (p == null || !value.HasValue) { return; }
                    p!.Xp = GameCalculator.experienceForLevel(value.Value);
                });

            emeralds = new MappedProperty<ProfileSaveFile?, ulong?>(_profile,
                p => p?.Currency.FirstOrDefault()?.Count,
                (p, value) =>
                {
                    if (p == null || value == null) { return; }
                    Currency currency = p!.Currency.FirstOrDefault() ?? new Currency() { Type = "Emerald" };
                    currency.Count = value.Value;
                    p!.Currency = (new[] { currency }).Concat(p!.Currency.Skip(1)).ToArray();
                });

            filteredItemList = new MappedProperty<ItemFilterEnum, IEnumerable<Item>>(_filter,
                f => {
                    var items = this.profile.value?.unequippedItems() ?? new Item[0];
                    return applyFilter(f, items).OrderBy(x => x.InventoryIndex!.Value).ToArray();
                });

            equippedItemList = new MappedProperty<ProfileSaveFile?, IEnumerable<Item>>(_profile, p => p?.equippedItems() ?? new Item[0]);

            profile.subscribe(p => this.filter.setValue = ItemFilterEnum.All);
        }

        #region Open File

        public async Task handleFileOpenAsync(string? filePath)
        {
            if (filePath == null) { return; }
            Console.WriteLine("Reading file: {0}", filePath!);
            if (Path.GetExtension(filePath!) == Constants.DECRYPTED_FILE_EXTENSION)
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
                EventLogger.logError($"The file \"{file.Name}\" was in an unexpected format.");
                showError?.Invoke(R.formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(file.Name));
                return;
            }
            using Stream? processed = await FileProcessHelper.Decrypt(inputStream);
            if (processed == null)
            {
                EventLogger.logError($"Content of file \"{file.Name}\" could not be converted to a supported format.");
                showError?.Invoke(R.formatFILE_DECRYPT_ERROR_MESSAGE(file.Name));
                return;
            }
            this.filePath = filePath;
            await tryParseFileStreamAsync(processed!);
        }

        private async Task tryParseFileStreamAsync(Stream stream)
        {
#if DEBUG
            var copy = new MemoryStream();
            await stream.CopyToAsync(copy);

            stream.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream);
            _profile.value = profile;
            _selectedItem.value = null;

            await verifyNoDataLossOnWrite(copy);
#else
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                var profile = await ProfileParser.Read(stream);
                _profile.value = profile;
                _selectedItem.value = null;
            }
            catch (Exception e)
            {
                EventLogger.logError(e.ToString());
                showError?.Invoke(R.FAILED_TO_PARSE_FILE_ERROR_MESSAGE);
            }
#endif
        }

        private async Task verifyNoDataLossOnWrite(Stream input)
        {
            var inputLines = getLinesFromJsonStream(input).ToArray();
            using var output = await ProfileParser.Write(_profile.value);
            var outputLines = getLinesFromJsonStream(output).ToArray();
            int lineIndex = 0;
            for (; lineIndex < Math.Min(inputLines.Length, outputLines.Length); lineIndex++)
            {
                if(!inputLines[lineIndex].Equals(outputLines[lineIndex]))
                {
                    break;
                }
            }

            if (lineIndex < inputLines.Length)
            {
                Console.WriteLine("Input:");
                Console.WriteLine("{0}:{1}", lineIndex, inputLines[lineIndex]);
            }
            if (lineIndex < outputLines.Length)
            {
                Console.WriteLine("Output:");
                Console.WriteLine("{0}:{1}", lineIndex, outputLines[lineIndex]);
            }
        }

        private IEnumerable<string> getLinesFromJsonStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamStr = stream.readAllText();
            var reader = new StringReader(Utilities.prettyJson(streamStr));
            return reader.readAllLines();
        }

        #endregion

        #region Save File

        public async Task handleFileSaveAsync(string? filePath)
        {
            if(filePath == null) { return; }
            Console.WriteLine("Writing file: {0}", filePath!);
            if (Path.GetExtension(filePath!) == Constants.DECRYPTED_FILE_EXTENSION)
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
                EventLogger.logError($"Failed to encrypt the json data.");
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
            var inventory = profile.value!.Items.ToList();
            var index = inventory.IndexOf(selectedItem.value!);

            inventory.RemoveAt(index);
            inventory.Insert(index, item);
            profile.value!.Items = inventory.ToArray();

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

    }
}
