using Microsoft.Win32;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class RegistryService : IRegistryService
{
    public void DeleteValue(string subKey, string name, bool strict)
    {
        using var regSubkey = Registry.CurrentUser.OpenSubKey(subKey, true);
        if (regSubkey is null && strict)
        {
            ThrowNoSubKeyException(subKey);
        }
        regSubkey?.DeleteValue(name, strict);
    }

    public object GetValue(string subKey, string name)
    {
        if (!TryGetValue(subKey, name, out var value))
        {
            ThrowNoSubKeyException(subKey);
        }
        return value ?? throw new ArgumentException($"No key named {name} in {subKey}");
    }

    public void SetValue(string subKey, string name, object value)
    {
        using var regSubkey = Registry.CurrentUser.OpenSubKey(subKey, true);
        if (regSubkey is null)
        {
            ThrowNoSubKeyException(subKey);
        }
        regSubkey!.SetValue(name, value);
    }

    public bool TryGetValue(string subKey, string name, out object? value)
    {
        using var regSubKey = Registry.CurrentUser.OpenSubKey(subKey, false);
        if (regSubKey is null)
        {
            value = null;
            return false;
        }
        value = regSubKey.GetValue(name);
        return value is not null;
    }

    private static void ThrowNoSubKeyException(string subKey)
    {
        throw new ArgumentException($"No sub key {subKey} in registry");
    }
}