namespace RandomWallpaper.Contracts;

public interface IRegistryService
{
    bool TryGetValue(string subKey, string name, out object? value);
    object GetValue(string subKey, string name);
    void SetValue(string subKey, string name, object value);
    void DeleteValue(string subKey, string name, bool strict);
}