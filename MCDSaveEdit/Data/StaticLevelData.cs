using MCDSaveEdit.Save.Models.Enums;
using System.Windows;

namespace MCDSaveEdit
{
    public struct StaticLevelData
    {
        public string key;
        public Point mapPosition;
        public LevelTypeEnum levelType;

        public StaticLevelData(string key, Point mapPosition, LevelTypeEnum levelType) : this()
        {
            this.key = key;
            this.mapPosition = mapPosition;
            this.levelType = levelType;
        }
    }
}
