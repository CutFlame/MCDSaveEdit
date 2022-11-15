using System;
#nullable enable

namespace MCDSaveEdit.Data
{
    public class ColorTuple : Tuple<byte, byte, byte, byte>
    {
        public ColorTuple(byte a, byte r, byte g, byte b) : base(a, r, g, b) { }
    }

    public class RectTuple : Tuple<int, int, int, int>
    {
        public RectTuple(int x, int y, int width, int height) : base(x, y, width, height) { }
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