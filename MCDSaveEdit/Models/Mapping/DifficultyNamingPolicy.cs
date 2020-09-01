using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class DifficultyNamingPolicy : INamingPolicy<Difficulty>
    {
        /// <inheritdoc />
        public Difficulty ConvertName(string name)
        {
            return name switch
            {
                "Difficulty_1" => Difficulty.Default,
                "Difficulty_2" => Difficulty.Adventure,
                "Difficulty_3" => Difficulty.Apocalypse,
                _ => Difficulty.Default,
            };
        }

        /// <inheritdoc />
        public string ConvertValue(Difficulty value)
        {
            return value switch
            {
                Difficulty.Default => "Difficulty_1",
                Difficulty.Adventure => "Difficulty_2",
                Difficulty.Apocalypse => "Difficulty_3",
                _ => "Difficulty_1",
            };
        }
    }
}
