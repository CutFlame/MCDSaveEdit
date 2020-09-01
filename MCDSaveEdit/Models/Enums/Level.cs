using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Enums
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [JsonConverter(typeof(CustomNamingEnumJsonConverter<Level, LowercaseNamingPolicy<Level>>))]
    public enum Level
    { // Naming: lowercase
        SquidCoast,
        CreeperWoods,
        PumpkinPastures,
        SoggySwamp,
        MoonCoreCaverns,
        FieryForge,
        DesertTemple,
        SlimySewers,
        HighBlockHalls,
        ObsidianPinnacle,
        CactiCanyon,
        CreeperWoodsDaily,
        PumpkinPasturesDaily,
        MoonCoreCavernsDaily,
        FieryForgeDaily,
        DesertTempleDaily,
        SlimySewersDaily,
        HighBlockHallsDaily,
        ObsidianPinnacleDaily,
        CactiCanyonDaily,
        SoggySwampDaily,
        CreepyCrypt, // Bonus Level
        SoggyCave, // Bonus Level
        UnderHalls, // Bonus Level
        ArchHaven, // Bonus Level
        LowerTemple, // Bonus Level
        MooshroomIsland, // Bonus Level
        DingyJungle, // Jungle DLC
        OvergrownTemple, // Jungle DLC
        BambooBluff, // Jungle DLC
        FrozenFjord, // Winter DLC
        LonelyFortress, // Winter DLC
        LostSettlement, // Winter DLC
    }
}
