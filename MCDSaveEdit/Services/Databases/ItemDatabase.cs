using System.Collections.Generic;

namespace MCDSaveEdit.Services
{
    public static class ItemDatabase
    {
        public static HashSet<string> all = new HashSet<string>();
        public static HashSet<string> artifacts = new HashSet<string>();
        public static HashSet<string> armor = new HashSet<string>();
        public static HashSet<string> meleeWeapons = new HashSet<string>();
        public static HashSet<string> rangedWeapons = new HashSet<string>();
        public static HashSet<string> armorProperties = new HashSet<string>();
    }
}
