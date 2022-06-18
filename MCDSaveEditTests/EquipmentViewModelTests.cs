using MCDSaveEdit;
using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
#nullable enable

namespace MCDSaveEditTests.ViewModelTests
{
    [TestClass]
    public class MainEquipmentViewModelTests
    {
        [TestMethod]
        public void TestConstructor()
        {
            var property = new Property<ProfileSaveFile?>(null);
            var instance = new MainEquipmentViewModel(property);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            Assert.AreEqual(true, instance.equipmentCanExist.value);
            Assert.AreEqual(Constants.MINIMUM_CHARACTER_LEVEL, instance.level.value);
            Assert.AreEqual(ItemFilterEnum.All, instance.filter.value);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddEquippedItems()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            instance.addEquippedItem(melee);
            var armor = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.ArmorGear);
            instance.addEquippedItem(armor);
            var ranged = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.RangedGear);
            instance.addEquippedItem(ranged);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(3, instance.equippedItemList.value.Count());
            Assert.AreEqual(1, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddEquippedItemsMultipleTimes()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            var armor = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.ArmorGear);
            var ranged = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.RangedGear);
            instance.addEquippedItem(melee);
            instance.addEquippedItem(armor);
            instance.addEquippedItem(ranged);

            var melee2 = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            melee2.Type = "AnotherSword";
            instance.addEquippedItem(melee2);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(3, instance.equippedItemList.value.Count());
            Assert.AreEqual(1, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddItemsToList()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForFilter(ItemFilterEnum.MeleeWeapons);
            instance.addItemToList(melee);
            var armor = Constants.createDefaultItemForFilter(ItemFilterEnum.Armor);
            instance.addItemToList(armor);
            var ranged = Constants.createDefaultItemForFilter(ItemFilterEnum.RangedWeapons);
            instance.addItemToList(ranged);
            Assert.AreEqual(3, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

    }

    [TestClass]
    public class StorageChestEquipmentViewModelTests
    {
        [TestMethod]
        public void TestConstructor()
        {
            var property = new Property<ProfileSaveFile?>(null);
            var instance = new StorageChestEquipmentViewModel(property);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new StorageChestEquipmentViewModel(property);
            Assert.AreEqual(ItemFilterEnum.All, instance.filter.value);
            Assert.AreEqual(true, instance.equipmentCanExist.value);
            Assert.AreEqual(Constants.MINIMUM_CHARACTER_LEVEL, instance.level.value);
            Assert.AreEqual(0, instance.remainingEnchantmentPoints.value);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddItemsToList()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new StorageChestEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForFilter(ItemFilterEnum.MeleeWeapons);
            instance.addItemToList(melee);
            var armor = Constants.createDefaultItemForFilter(ItemFilterEnum.Armor);
            instance.addItemToList(armor);
            var ranged = Constants.createDefaultItemForFilter(ItemFilterEnum.RangedWeapons);
            instance.addItemToList(ranged);
            Assert.AreEqual(3, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }


    }
}
