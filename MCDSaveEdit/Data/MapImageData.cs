using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public struct MapImageData
    {
        public string titleLookupString;
        public string titleBackupText;
        public string mapImageSourcePath;
        public Color backgroundColor;
        public Int32Rect? cropToRect;
        public StaticLevelData[] levelData;

        public BitmapSource? mapImageSource;

        public string title()
        {
            return R.getString(titleLookupString) ?? titleBackupText;
        }

        public void preload()
        {
            this.mapImageSource = AppModel.instance.imageSource(mapImageSourcePath);
        }

        public ImageSource? usableImageSource()
        {
            if(this.mapImageSource == null)
            {
                preload();
            }

            if (this.mapImageSource != null && this.cropToRect.HasValue)
            {
                return new CroppedBitmap(this.mapImageSource!, this.cropToRect.Value);
            }
            else
            {
                return this.mapImageSource;
            }
        }
    }
}
