using MCDSaveEdit.Save.Models.Enums;
using System.Numerics;

namespace MCDSaveEdit.Data
{

    public struct StaticLevelData
    {
        public string key;
        public Vector2 mapPosition;
        public LevelTypeEnum levelType;
        public bool alwaysAvailable;

        public StaticLevelData(string key, Vector2 mapPosition, LevelTypeEnum levelType, bool alwaysAvailable = false)
        {
            this.key = key;
            this.mapPosition = mapPosition;
            this.levelType = levelType;
            this.alwaysAvailable = alwaysAvailable;
        }
    }
}
