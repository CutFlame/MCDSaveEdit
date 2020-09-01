using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Enums
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [JsonConverter(typeof(CustomNamingEnumJsonConverter<HintType, HintTypeNamingPolicy>))]
    public enum HintType
    {
        Movement,
        Objective,
        DefeatZombie,
        LowHealth,
        PickupArrow,
        RangedAttack,
        ArtifactPickup, // Original: Artifact_Pickup
        ArtifactOpenInventory, // Original: Artifact_OpenInventory
        ArtifactEquip, // Original: Artifact_Equip
        ArtifactActivate, // Original: Artifact_Activate
        ArtifactRangedAttack, // Original: Artifact_RangedAttack
        EnchantingOpenInventory, // Original: Enchanting_OpenInventory
        EnchantingSelectGear, // Original: Enchanting_SelectGear
        EnchantingSelectEnchantment, // Original: Enchanting_SelectEnchantment
        EnchantingItemEnchanted, // Original: Enchanting_ItemEnchanted
        MissionSelectMarker, // Original: MissionSelect_Marker
        MissionSelectPopup, // Original: MissionSelect_Popup
        MerchantsFindMerchants, // Original: Merchants_FindMerchants
        MerchantsInteract, // Original: Merchants_Interact
        Map,
        Teleport,
        Popping,
        ChatWheelOpen, // Original: ChatWheel_Open
        ChatWheelSelect, // Original: ChatWheel_Select
        ChatWheelChat, // Original: ChatWheel_Chat
    }
}
