using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [JsonConverter(typeof(CustomNamingEnumJsonConverter<Threat, ThreatNamingPolicy>))]
    public enum Threat
    {
        I, // Original: Threat_1
        II, // Original: Threat_2
        III, // Original: Threat_3
        IV, // Original: Threat_4
        V, // Original: Threat_5
        VI, // Original: Threat_6
    }
}
