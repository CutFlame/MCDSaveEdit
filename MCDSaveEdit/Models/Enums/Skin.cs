using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Enums
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [JsonConverter(typeof(CustomNamingEnumJsonConverter<Skin, SnakeCaseNamingPolicy<Skin>>))]
    public enum Skin
    { // Naming: snake_case
        Alex,
        Annika,
        Archeologist, // Jungle DLC
        Baako,
        Bediako,
        Darian,
        Elaine,
        Eshe,
        Esperanza,
        Explorer, // Jungle DLC
        Frosty, // Winter DLC
        Fuego,
        Greta,
        Hal,
        Hedwig,
        Hex,
        Igor,
        Jade,
        Mayeso,
        Morris,
        Neo,
        Nuru,
        Pake, // Hero Edition
        Qamar,
        Sam,
        Sergey,
        Shikoba,
        Steve,
        Sven,
        Valorie,
        Violet,
        Wargen, // Hero Edition
        WinterWarrior, // Winter DLC; Original: winter_warrior
        Zola,
    }
}
