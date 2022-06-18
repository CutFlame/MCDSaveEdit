using MCDSaveEdit.Interfaces;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class LanguageResolver
    {
        public static ILanguageResolver instance = new LocalContentResolver();
    }
}
