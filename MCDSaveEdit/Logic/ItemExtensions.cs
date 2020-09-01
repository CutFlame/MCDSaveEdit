using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;

namespace MCDSaveEdit.Save.Models.Enums
{
    public static class ItemExtensions
    {
        public static int level(this Item item)
        {
            return GameCalculator.levelFromPower(item.Power);
        }

        public static int enchantmentPoints(this Item item)
        {
            if (item.Enchantments == null) { return 0; }
            int points = 0;
            foreach (var enchantment in item.Enchantments)
            {
                points += enchantment.pointsCost();
            }
            return points;
        }

        public static ItemTypeEnum type(this Item item)
        {
            return itemTypeFromItemName(item.Name);
        }

        private static ItemTypeEnum itemTypeFromItemName(string itemName)
        {
            //exceptions where the name in the file doesn't match the name on the images
            if (itemName == "Sword")
            {
                itemName = "Sword_Steel";
            }
            if (itemName == "Pickaxe")
            {
                itemName = "Pickaxe_Steel";
            }
            if (itemName == "Pickaxe_Unique1")
            {
                itemName = "Pickaxe_Unique1_Steel";
            }

            if(Enum.TryParse(itemName, true, out ItemTypeEnum result))
            {
                return result;
            }

            return ItemTypeEnum.Unknown;
        }

        public static bool isArtifact(this Item item)
        {
            return item.type().isArtifact();
        }
        public static bool isArtifact(this ItemTypeEnum itemType)
        {
            return artifacts.Contains(itemType);
        }

        public static bool isArmor(this Item item)
        {
            return item.type().isArmor();
        }
        public static bool isArmor(this ItemTypeEnum itemType)
        {
            return armor.Contains(itemType);
        }

        public static bool isMeleeWeapon(this Item item)
        {
            return item.type().isMeleeWeapon();
        }
        public static bool isMeleeWeapon(this ItemTypeEnum itemType)
        {
            return meleeWeapons.Contains(itemType);
        }

        public static bool isRangedWeapon(this Item item)
        {
            return item.type().isRangedWeapon();
        }
        public static bool isRangedWeapon(this ItemTypeEnum itemType)
        {
            return rangedWeapons.Contains(itemType);
        }

        public static bool isInPatch1(this Item item)
        {
            return item.type().isInPatch1();
        }
        public static bool isInPatch1(this ItemTypeEnum itemType)
        {
            return patch1Items.Contains(itemType);
        }

        public static HashSet<ItemTypeEnum> patch1Items = new HashSet<ItemTypeEnum>() {
            ItemTypeEnum.Battlestaff,
            ItemTypeEnum.Battlestaff_Unique1,
            ItemTypeEnum.Battlestaff_Unique2,
            ItemTypeEnum.DualCrossbows,
            ItemTypeEnum.DualCrossbows_Unique1,
            ItemTypeEnum.DualCrossbows_Unique2,
        };

        public static HashSet<ItemTypeEnum> artifacts = new HashSet<ItemTypeEnum>() {
            ItemTypeEnum.IronHideAmulet,
            ItemTypeEnum.BootsOfSwiftness,
            ItemTypeEnum.CorruptedBeacon,
            ItemTypeEnum.CorruptedSeeds,
            ItemTypeEnum.DeathCapMushroom,
            ItemTypeEnum.DiamondDust,
            ItemTypeEnum.FireworksArrowItem,
            ItemTypeEnum.FishingRod,
            ItemTypeEnum.FlamingQuiver,
            ItemTypeEnum.GhostCloak,
            ItemTypeEnum.GolemKit,
            ItemTypeEnum.GongOfWeakening,
            ItemTypeEnum.Harvester,
            ItemTypeEnum.IceWand,
            ItemTypeEnum.LightFeather,
            ItemTypeEnum.LightningRod,
            ItemTypeEnum.LoveMedallion,
            ItemTypeEnum.ShockPowder,
            ItemTypeEnum.SoulHealer,
            ItemTypeEnum.TastyBone,
            ItemTypeEnum.TNTBox,
            ItemTypeEnum.TormentQuiver,
            ItemTypeEnum.TotemOfRegeneration,
            ItemTypeEnum.TotemOfShielding,
            ItemTypeEnum.TotemOfSoulProtection,
            ItemTypeEnum.WindHorn,
            ItemTypeEnum.WonderfulWheat,
        };

        public static HashSet<ItemTypeEnum> armor = new HashSet<ItemTypeEnum>() {
            ItemTypeEnum.ArchersStrappings,
            ItemTypeEnum.ArchersStrappings_Unique1,
            ItemTypeEnum.AssassinArmor,
            ItemTypeEnum.AssassinArmor_Unique1,
            ItemTypeEnum.BattleRobe,
            ItemTypeEnum.BattleRobe_Unique1,
            ItemTypeEnum.ChampionsArmor,
            ItemTypeEnum.ChampionsArmor_Unique1,
            ItemTypeEnum.CowardsArmor,
            ItemTypeEnum.CowardsArmor_Unique1,
            ItemTypeEnum.DarkArmor,
            ItemTypeEnum.DarkArmor_Unique1,
            ItemTypeEnum.EvocationRobe,
            ItemTypeEnum.EvocationRobe_Unique1,
            ItemTypeEnum.FullPlateArmor,
            ItemTypeEnum.FullPlateArmor_Unique1,
            ItemTypeEnum.GrimArmor,
            ItemTypeEnum.GrimArmor_Unique1,
            ItemTypeEnum.MercenaryArmor,
            ItemTypeEnum.MercenaryArmor_Unique1,
            ItemTypeEnum.MysteryArmor,
            ItemTypeEnum.OcelotArmor,
            ItemTypeEnum.OcelotArmor_Unique1,
            ItemTypeEnum.PhantomArmor,
            ItemTypeEnum.PhantomArmor_Unique1,
            ItemTypeEnum.ReinforcedMail,
            ItemTypeEnum.ReinforcedMail_Unique1,
            ItemTypeEnum.ScaleMail,
            ItemTypeEnum.ScaleMail_Unique1,
            ItemTypeEnum.SnowArmor,
            ItemTypeEnum.SnowArmor_Unique1,
            ItemTypeEnum.SoulRobe,
            ItemTypeEnum.SoulRobe_Unique1,
            ItemTypeEnum.SpelunkersArmor,
            ItemTypeEnum.SpelunkersArmor_Unique1,
            ItemTypeEnum.WolfArmor,
            ItemTypeEnum.WolfArmor_Unique1,
        };

        public static HashSet<ItemTypeEnum> meleeWeapons = new HashSet<ItemTypeEnum>()
        {
            ItemTypeEnum.Axe,
            ItemTypeEnum.Axe_Unique1,
            ItemTypeEnum.Axe_Unique2,
            ItemTypeEnum.Battlestaff,
            ItemTypeEnum.Battlestaff_Unique1,
            ItemTypeEnum.Battlestaff_Unique2,
            ItemTypeEnum.Claymore,
            ItemTypeEnum.Claymore_Unique1,
            ItemTypeEnum.Claymore_Unique2,
            ItemTypeEnum.Cutlass,
            ItemTypeEnum.Cutlass_Unique1,
            ItemTypeEnum.Cutlass_Unique2,
            ItemTypeEnum.Daggers,
            ItemTypeEnum.Daggers_Unique1,
            ItemTypeEnum.Daggers_Unique2,
            ItemTypeEnum.DoubleAxe,
            ItemTypeEnum.DoubleAxe_Unique1,
            ItemTypeEnum.DoubleAxe_Unique2,
            ItemTypeEnum.Gauntlets,
            ItemTypeEnum.Gauntlets_Unique1,
            ItemTypeEnum.Gauntlets_Unique2,
            ItemTypeEnum.Gauntlets_Unique3,
            ItemTypeEnum.Glaive,
            ItemTypeEnum.Glaive_Unique1,
            ItemTypeEnum.Glaive_Unique2,
            ItemTypeEnum.Hammer,
            ItemTypeEnum.Hammer_Unique1,
            ItemTypeEnum.Hammer_Unique2,
            ItemTypeEnum.HighlanderLongSword,
            ItemTypeEnum.Katana,
            ItemTypeEnum.Katana_Unique1,
            ItemTypeEnum.Katana_Unique2,
            ItemTypeEnum.Mace,
            ItemTypeEnum.Mace_Unique1,
            ItemTypeEnum.Mace_Unique2,
            ItemTypeEnum.Pickaxe_Steel,
            ItemTypeEnum.Pickaxe_Unique1_Steel,
            ItemTypeEnum.Punch,
            ItemTypeEnum.Sickles,
            ItemTypeEnum.Sickles_Unique1,
            ItemTypeEnum.Sickles_Unique2,
            ItemTypeEnum.SoulKnife,
            ItemTypeEnum.SoulKnife_Unique1,
            ItemTypeEnum.SoulKnife_Unique2,
            ItemTypeEnum.SoulScythe,
            ItemTypeEnum.SoulScythe_Unique1,
            ItemTypeEnum.SoulScythe_Unique2,
            ItemTypeEnum.Spear,
            ItemTypeEnum.Spear_Unique1,
            ItemTypeEnum.Spear_Unique2,
            ItemTypeEnum.Sword_Steel,
            ItemTypeEnum.Sword_Unique1,
            ItemTypeEnum.Sword_Unique2,
            ItemTypeEnum.Whip,
            ItemTypeEnum.Whip_Unique1,
        };

        public static HashSet<ItemTypeEnum> rangedWeapons = new HashSet<ItemTypeEnum>()
        {
            ItemTypeEnum.Bow,
            ItemTypeEnum.Bow_Unique1,
            ItemTypeEnum.Bow_Unique2,
            ItemTypeEnum.Crossbow,
            ItemTypeEnum.Crossbow_Unique1,
            ItemTypeEnum.Crossbow_Unique2,
            ItemTypeEnum.DualCrossbows,
            ItemTypeEnum.DualCrossbows_Unique1,
            ItemTypeEnum.DualCrossbows_Unique2,
            ItemTypeEnum.ExplodingCrossbow,
            ItemTypeEnum.ExplodingCrossbow_Unique1,
            ItemTypeEnum.ExplodingCrossbow_Unique2,
            ItemTypeEnum.HeavyCrossbow,
            ItemTypeEnum.HeavyCrossbow_Unique1,
            ItemTypeEnum.HeavyCrossbow_Unique2,
            ItemTypeEnum.HuntingBow,
            ItemTypeEnum.HuntingBow_Unique1,
            ItemTypeEnum.HuntingBow_Unique2,
            ItemTypeEnum.LongBow,
            ItemTypeEnum.LongBow_Unique1,
            ItemTypeEnum.LongBow_Unique2,
            ItemTypeEnum.PowerBow,
            ItemTypeEnum.PowerBow_Unique1,
            ItemTypeEnum.PowerBow_Unique2,
            ItemTypeEnum.RapidCrossbow,
            ItemTypeEnum.RapidCrossbow_Unique1,
            ItemTypeEnum.RapidCrossbow_Unique2,
            ItemTypeEnum.ScatterCrossbow,
            ItemTypeEnum.ScatterCrossbow_Unique1,
            ItemTypeEnum.ScatterCrossbow_Unique2,
            ItemTypeEnum.Shortbow,
            ItemTypeEnum.ShortBow_Unique1,
            ItemTypeEnum.ShortBow_Unique2,
            ItemTypeEnum.SlowBow,
            ItemTypeEnum.Slowbow_Unique1,
            ItemTypeEnum.SoulBow,
            ItemTypeEnum.SoulBow_Unique1,
            ItemTypeEnum.SoulBow_Unique2,
            ItemTypeEnum.SoulCrossbow,
            ItemTypeEnum.SoulCrossbow_Unique1,
            ItemTypeEnum.SoulCrossbow_Unique2,
            ItemTypeEnum.TrickBow,
            ItemTypeEnum.TrickBow_Unique1,
            ItemTypeEnum.TrickBow_Unique2,

        };
    }
}
