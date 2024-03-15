using Microsoft.Win32;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class RegistryService : IRegistryService
{
    public void DeleteValue(string subKey, string name, bool strict)
    {
        using var regSubkey = Registry.CurrentUser.OpenSubKey(subKey, true)!;
        regSubkey.DeleteValue(name, strict);
    }

    public void SetValue(string subKey, string name, object value)
    {
        using var regSubkey = Registry.CurrentUser.OpenSubKey(subKey, true)!;
        regSubkey.SetValue(name, value);
    }

    public bool TryGetValue(string subKey, string name, out object? value)
    {
        using var regSubKey = Registry.CurrentUser.OpenSubKey(subKey, false)!;
        value = regSubKey.GetValue(name);
        return value is not null;
    }
}