using System;

namespace MCDSaveEdit.Save.Models.Enums
{
    public enum DifficultyEnum
    {
        Difficulty_1,
        Difficulty_2,
        Difficulty_3,
    }

    public static class DifficultyEnumExtensions
    {
        public static int getValue(this DifficultyEnum difficultyEnum)
        {
            switch (difficultyEnum)
            {
                case DifficultyEnum.Difficulty_1: return 1;
                case DifficultyEnum.Difficulty_2: return 2;
                case DifficultyEnum.Difficulty_3: return 3;
            }
            throw new NotImplementedException();
        }

    }
}
