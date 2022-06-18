using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
#nullable enable

namespace MCDSaveEdit.Logic
{
    public static class ProgressExtensions
    {
        public static uint getDifficultyImageLevel(this Progress levelProgress)
        {
            if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_1.ToString())
            {
                return 2;
            }
            else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_2.ToString())
            {
                return 3;
            }
            else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_3.ToString() && levelProgress.CompletedEndlessStruggle <= 0)
            {
                return 4;
            }
            else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_3.ToString() && levelProgress.CompletedEndlessStruggle > 0)
            {
                return 5;
            }
            return 1;
        }
    }
}
