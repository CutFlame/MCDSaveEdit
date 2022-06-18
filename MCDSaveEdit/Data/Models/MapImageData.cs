using System;
#nullable enable

namespace MCDSaveEdit.Data
{
    public class ColorTuple : Tuple<byte, byte, byte, byte>
    {
        public ColorTuple(byte one, byte two, byte three, byte four) : base(one, two, three, four) { }
    }

    public class RectTuple : Tuple<int, int, int, int>
    {
        public RectTuple(int one, int two, int three, int four) : base(one, two, three, four) { }
    }

    public struct MapImageData
    {
        public string titleLookupString;
        public string titleBackupText;
        public string mapImageSourcePath;
        public ColorTuple backgroundColor;
        public RectTuple? cropToRect;
        public StaticLevelData[] levelData;
    }
}