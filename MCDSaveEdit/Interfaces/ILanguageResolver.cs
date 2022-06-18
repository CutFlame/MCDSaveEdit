using System.Collections.Generic;
#nullable enable

namespace MCDSaveEdit.Interfaces
{
    public interface ILanguageResolver
    {
        string? path { get; }
        IReadOnlyCollection<string> localizationOptions { get; }
        Dictionary<string, Dictionary<string, string>>? loadLanguageStrings(string langSpecifier);

    }
}
