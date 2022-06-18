using MCDSaveEdit.Interfaces;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class ImageResolver
    {
        public static IImageResolver instance = new LocalContentResolver();
    }
}
