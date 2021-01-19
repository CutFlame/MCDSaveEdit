using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public struct MapImageData
    {
        public string title;
        public string mapImageSourcePath;
        public Color backgroundColor;
        public Int32Rect? cropToRect;
        public BitmapSource? mapImageSource;

        public void preload()
        {
            this.mapImageSource = ImageUriHelper.instance.imageSource(mapImageSourcePath);
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
