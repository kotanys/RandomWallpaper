using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class IOService : IIOService
{
    public string ProcessPath => Environment.ProcessPath!;

    public IEnumerable<string> EnumerateFiles(string directory) => Directory.EnumerateFiles(directory);

    public bool FileExists(string file) => File.Exists(file);

    public string GetAbsolutePath(string path) => Path.GetFullPath(path);

}
