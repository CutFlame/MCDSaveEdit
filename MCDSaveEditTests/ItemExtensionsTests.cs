using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCDSaveEditTests.LogicTests
{
    [TestClass]
    public class ItemExtensionsTests
    {
        [TestMethod]
        public void ReplaceArmorPropertyChangesOnlySelectedDuplicate()
        {
            var first = new Armorproperty { Id = "duplicate", Rarity = Rarity.Common };
            var second = new Armorproperty { Id = "duplicate", Rarity = Rarity.Unique };
            var item = new Item { Armorproperties = new[] { first, second } };

            Assert.IsTrue(item.replaceArmorProperty(second, "replacement"));

            Assert.AreEqual("duplicate", first.Id);
            Assert.AreEqual("replacement", second.Id);
            Assert.AreEqual(Rarity.Unique, second.Rarity);
            Assert.AreEqual(2, item.Armorproperties.Length);
            Assert.AreSame(first, item.Armorproperties[0]);
            Assert.AreSame(second, item.Armorproperties[1]);
        }

        [TestMethod]
        public void ReplaceArmorPropertyIgnoresEntryRemovedWhileSelectorIsOpen()
        {
            var removed = new Armorproperty { Id = "duplicate" };
            var remaining = new Armorproperty { Id = "duplicate" };
            var item = new Item { Armorproperties = new[] { remaining } };

            Assert.IsFalse(item.replaceArmorProperty(removed, "replacement"));
            Assert.AreEqual("duplicate", remaining.Id);
            Assert.AreEqual("duplicate", removed.Id);
        }

        [TestMethod]
        public void ReplaceArmorPropertyIgnoresEntryFromAnotherItem()
        {
            var original = new Armorproperty { Id = "duplicate" };
            var otherItem = new Item { Armorproperties = new[] { original.Copy() } };

            Assert.IsFalse(otherItem.replaceArmorProperty(original, "replacement"));
            Assert.AreEqual("duplicate", original.Id);
            Assert.AreEqual("duplicate", otherItem.Armorproperties[0].Id);
        }

        [TestMethod]
        public void ReplaceArmorPropertyIgnoresCancelledSelection()
        {
            var property = new Armorproperty { Id = "original" };
            var item = new Item { Armorproperties = new[] { property } };

            Assert.IsFalse(item.replaceArmorProperty(property, null));
            Assert.AreEqual("original", property.Id);
        }

        [TestMethod]
        public void ReplaceArmorPropertyHandlesMissingOrEmptyProperties()
        {
            var property = new Armorproperty { Id = "original" };
            var item = new Item();

            Assert.IsFalse(item.replaceArmorProperty(property, "replacement"));
            item.Armorproperties = new Armorproperty[0];
            Assert.IsFalse(item.replaceArmorProperty(property, "replacement"));
            Assert.AreEqual("original", property.Id);
        }
    }
}
