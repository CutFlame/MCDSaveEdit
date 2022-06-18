using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Class;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.Services
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
            var bitmap = bitmapImageFromSKImage(texture.Image);
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

        public static BitmapImage bitmapImageFromSKBitmap(SKBitmap image) => bitmapImageFromSKImage(SKImage.FromBitmap(image));
        public static BitmapImage bitmapImageFromSKImage(SKImage image)
        {
            using var encoded = image.Encode();
            using var stream = encoded.AsStream();
            BitmapImage photo = new BitmapImage();
            photo.BeginInit();
            photo.CacheOption = BitmapCacheOption.OnLoad;
            photo.StreamSource = stream;
            photo.EndInit();
            photo.Freeze();
            return photo;
        }


        /// <summary>
        /// Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
        /// </summary>
        /// <param name="pathInApplication">Path without starting slash</param>
        private static BitmapImage loadBitmapFromResource(string pathInApplication)
        {
            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            var rootPath = @"pack://application:,,/";
            var fullPath = Path.Combine(rootPath, pathInApplication);
            var uri = new Uri(fullPath, UriKind.RelativeOrAbsolute);
            Console.WriteLine(uri);
            return new BitmapImage(uri);
        }

    }
}
