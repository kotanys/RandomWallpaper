using RandomWallpaper.Contracts;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class RegistryAutorunServiceTests
{
    const string Key = "Test key";
    const string AutorunSubkey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private readonly MockRegistryService mockRegistry = new();
    private readonly RegistryAutorunService autorunService;
    public RegistryAutorunServiceTests()
    {
        autorunService = new RegistryAutorunService(new MockIOService(), mockRegistry, new MockLogger(), Key);
    }

    [Fact]
    public void ShouldAddCorrectKey()
    {
        autorunService.Add("now .wpcfg");
        Assert.True((string)mockRegistry.Keys[AutorunSubkey + "\\" + Key] == (Environment.ProcessPath + " now .wpcfg"));
    }

    [Fact]
    public void ShouldRemoveCorrentKey()
    {
        mockRegistry.Keys[AutorunSubkey + "\\" + Key] = Environment.ProcessPath + " now";
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
            throw new NotImplementedException();
        }
    }

    private class MockIOService : IIOService
    {
        public string ProcessPath => Environment.ProcessPath!;

        public IEnumerable<string> EnumerateFiles(string directory)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string file)
        {
            throw new NotImplementedException();
        }

        public string GetAbsolutePath(string path)
        {
            throw new NotImplementedException();
        }
    }

    private class MockLogger : ILoggerService
    {
        public void LogError(string message) {}

        public void LogInformation(string message) {}
    }
}