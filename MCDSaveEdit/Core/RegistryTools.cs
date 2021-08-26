using Microsoft.Win32;
#nullable enable

namespace MCDSaveEdit
{
    public class RegistryTools
    {
        // Save a value.
        public static void SaveSetting(string app_name, string name, object value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            sub_key.SetValue(name, value);
        }

        // Get a value.
        public static T GetSetting<T>(string app_name, string name, T default_value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            return (T)sub_key.GetValue(name, default_value);
        }

        // Delete a value.
        public static void DeleteSetting(string app_name, string name)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            try
            {
                sub_key.DeleteValue(name);
            }
            catch
            {
                //Intentionally left blank to ignore errors
            }
        }
    }
}
