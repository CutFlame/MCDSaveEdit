using System;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class HintTypeNamingPolicy : INamingPolicy<HintType>
    {
        /// <inheritdoc />
        public HintType ConvertName(string name)
        {
            //return Enum.Parse<HintType>(name.Contains('_') ? name.Replace("_", "") : name);
            return (HintType)Enum.Parse(typeof(HintType), name.Contains("_") ? name.Replace("_", "") : name);
        }

        /// <inheritdoc />
        public string ConvertValue(HintType value)
        {
            return value switch
            {
                HintType.ArtifactPickup => "Artifact_Pickup",
                HintType.ArtifactOpenInventory => "Artifact_OpenInventory",
                HintType.ArtifactEquip => "Artifact_Equip",
                HintType.ArtifactActivate => "Artifact_Activate",
                HintType.ArtifactRangedAttack => "Artifact_RangedAttack",
                HintType.EnchantingOpenInventory => "Enchanting_OpenInventory",
                HintType.EnchantingSelectGear => "Enchanting_SelectGear",
                HintType.EnchantingSelectEnchantment => "Enchanting_SelectEnchantment",
                HintType.EnchantingItemEnchanted => "Enchanting_ItemEnchanted",
                HintType.MissionSelectMarker => "MissionSelect_Marker",
                HintType.MissionSelectPopup => "MissionSelect_Popup",
                HintType.MerchantsFindMerchants => "Merchants_FindMerchants",
                HintType.MerchantsInteract => "Merchants_Interact",
                HintType.ChatWheelOpen => "ChatWheel_Open",
                HintType.ChatWheelSelect => "ChatWheel_Select",
                HintType.ChatWheelChat => "ChatWheel_Chat",
                _ => typeof(HintType).GetEnumName(value)!,
            };
        }
    }
}
