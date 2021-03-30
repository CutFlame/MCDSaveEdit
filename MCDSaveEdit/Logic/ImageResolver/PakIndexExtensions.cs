using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Class;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public static class PakIndexExtensions
    {
        public static PakPackage? extractPackage(this PakIndex pakIndex, string fullPath)
        {
            if (!pakIndex.TryGetPackage(fullPath, out var package))
            {
                EventLogger.logError($"Could not get package from {fullPath}");
                return null;
            }
            if (!package.HasExport())
            {
                EventLogger.logError($"Package does not have export {fullPath}");
                return null;
            }
            return package;
        }

        public static BitmapImage? extractBitmap(this PakIndex pakIndex, string fullPath)
        {
            var package = pakIndex.extractPackage(fullPath);
            var texture = package?.GetExport<UTexture2D>();
            if (texture == null)
            {
                EventLogger.logError($"Could not get texture from package {fullPath}");
                return null;
            }
            var bitmap = AppModel.bitmapImageFromSKImage(texture.Image);
            if (bitmap == null)
            {
                EventLogger.logError($"Could not get bitmap from texture {fullPath}");
                return null;
            }
            bitmap.Freeze();
            return bitmap;
        }

        public static Dictionary<string, Dictionary<string, string>>? extractLocResFile(this PakIndex pakIndex, string fullPath)
        {
            if (!pakIndex.TryGetFile(fullPath, out var byteArray) || byteArray == null)
            {
                EventLogger.logError($"Could not get anything from {fullPath}");
                return null;
            }
            var stream = new MemoryStream(byteArray!.Value.Array, byteArray!.Value.Offset, byteArray!.Value.Count);
            Dictionary<string, Dictionary<string, string>>? entries = new LocResReader(stream).Entries;
            return entries;
        }
    }
}
