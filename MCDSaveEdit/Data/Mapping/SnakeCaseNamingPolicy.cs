using System;
using System.Linq;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class SnakeCaseNamingPolicy<T> : INamingPolicy<T> where T : Enum
    {
        /// <inheritdoc />
        public T ConvertName(string name)
        {
            return (T)Enum.Parse(typeof(T), name.Replace("_", ""), true);
        }

        /// <inheritdoc />
        public string ConvertValue(T value)
        {
            string enumString = typeof(T).GetEnumName(value)!;
            return string.Concat(enumString.Select((c, i) => char.IsUpper(c) ? (i == 0 ? "" : "_") + char.ToLowerInvariant(c) : c.ToString()));
        }
    }
}
