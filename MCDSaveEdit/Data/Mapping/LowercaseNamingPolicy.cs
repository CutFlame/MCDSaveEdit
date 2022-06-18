using System;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class LowercaseNamingPolicy<T> : INamingPolicy<T> where T : Enum
    {
        /// <inheritdoc />
        public T ConvertName(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }

        /// <inheritdoc />
        public string ConvertValue(T value)
        {
            string enumString = typeof(T).GetEnumName(value)!;
            return enumString.ToLowerInvariant();
        }
    }
}
