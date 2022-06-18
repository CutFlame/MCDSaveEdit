using MCDSaveEdit.Data;
using MCDSaveEdit.Services;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.Logic
{
    public static class MapImageDataExtensions {
        public static string title(this MapImageData data)
        {
            return R.getString(data.titleLookupString) ?? data.titleBackupText;
        }

        public static void preload(this MapImageData data)
        {
            _ = ImageResolver.instance.imageSource(data.mapImageSourcePath);
        }

        public static ImageSource? usableImageSource(this MapImageData data)
        {
            var imageSource = ImageResolver.instance.imageSource(data.mapImageSourcePath);

            if (imageSource != null && data.cropToRect != null)
            {
                var tuple = data.cropToRect;
                var rect = new Int32Rect(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
                return new CroppedBitmap(imageSource!, rect);
            }
            else
            {
                return imageSource;
            }
        }
    }
}
