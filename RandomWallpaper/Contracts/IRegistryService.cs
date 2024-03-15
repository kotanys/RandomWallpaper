namespace RandomWallpaper.Contracts;

public interface IRegistryService
{
    bool TryGetValue(string subKey, string name, out object? value);
    object GetValue(string subKey, string name)
    {
        if (!TryGetValue(subKey, name, out var value) || value is null)
        {
            throw new ArgumentException($"Value {subKey}\\{name} doesn't exist in registry");
        }
        return value;
    }
    void SetValue(string subKey, string name, object value);
    void DeleteValue(string subKey, string name, bool strict);
}