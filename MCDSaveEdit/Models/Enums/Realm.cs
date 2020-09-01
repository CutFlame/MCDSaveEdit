using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Enums
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    [JsonConverter(typeof(CustomNamingEnumJsonConverter<Realm, RealmNamingPolicy>))]
    public enum Realm
    {
        ArchIllager, // Original: ArchIllagerRealm
        Islands, // Jungle DLC; Winter DLC; Original: IslandsRealm
    }
}
