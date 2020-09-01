using System;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class RealmNamingPolicy : INamingPolicy<Realm>
    {
        /// <inheritdoc />
        public Realm ConvertName(string name)
        {
            return (Realm)Enum.Parse(typeof(Realm), name.Replace("Realm", ""), true);
        }

        /// <inheritdoc />
        public string ConvertValue(Realm value)
        {
            string enumString = typeof(Realm).GetEnumName(value)!;
            return $"{enumString}Realm";
        }
    }
}
