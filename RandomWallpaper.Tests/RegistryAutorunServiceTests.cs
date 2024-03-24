using Moq;
using RandomWallpaper.Contracts;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class RegistryAutorunServiceTests
{
    const string Key = "Test key";
    const string AutorunSubkey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string MockAppdir = @"C:\Appdir\RandomWallpaper.exe";

    private readonly MockRegistryService mockRegistry = new();
    private readonly RegistryAutorunService autorunService;
    public RegistryAutorunServiceTests()
    {
        var mockIO = Mock.Of<IIOService>(io => io.ProcessPath == MockAppdir);
        var mockLogger = Mock.Of<ILoggerService>();
        autorunService = new RegistryAutorunService(mockIO, mockRegistry, mockLogger, Key);
    }

    [Fact]
    public void ShouldAddCorrectKey()
    {
        autorunService.Add("now .wpcfg");
        Assert.True((string)mockRegistry.Keys[AutorunSubkey + "\\" + Key] == (MockAppdir + " now .wpcfg"));
    }

    [Fact]
    public void ShouldRemoveCorrentKey()
    {
        mockRegistry.Keys[AutorunSubkey + "\\" + Key] = MockAppdir + " now";
        autorunService.Remove();
        Assert.True(mockRegistry.Keys.Count == 0);
    }

    private class MockRegistryService : IRegistryService
    {
        public Dictionary<string, object> Keys {get;} = [];

        public void DeleteValue(string subKey, string name, bool strict)
        {
            if (strict && !Keys.ContainsKey(subKey + "\\" + name))
            {
                throw new InvalidOperationException("Tried to delete a key that doesn't exist");
            }
            Keys.Remove(subKey + "\\" + name);
        }

        public void SetValue(string subKey, string name, object value)
        {
            Keys[subKey + "\\" + name] = value;
        }

        public bool TryGetValue(string subKey, string name, out object? value)
        {
            value = Keys[subKey + "\\" + name];
            return true;
        }
    }
}