namespace MCDSaveEdit.Save.Models.Mapping
{
    public interface INamingPolicy<T>
    {
        public T ConvertName(string name);
        public string ConvertValue(T value);
    }
}
