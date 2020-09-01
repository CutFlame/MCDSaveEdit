using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class ThreatNamingPolicy : INamingPolicy<Threat>
    {
        /// <inheritdoc />
        public Threat ConvertName(string name)
        {
            return name switch
            {
                "Threat_1" => Threat.I,
                "Threat_2" => Threat.II,
                "Threat_3" => Threat.III,
                "Threat_4" => Threat.IV,
                "Threat_5" => Threat.V,
                "Threat_6" => Threat.VI,
                _ => Threat.I,
            };
        }

        /// <inheritdoc />
        public string ConvertValue(Threat value)
        {
            return value switch
            {
                Threat.I => "Threat_1",
                Threat.II => "Threat_2",
                Threat.III => "Threat_3",
                Threat.IV => "Threat_4",
                Threat.V => "Threat_5",
                Threat.VI => "Threat_6",
                _ => "Threat_1",
            };
        }
    }
}
