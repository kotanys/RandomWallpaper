namespace RandomWallpaper.Contracts;

public interface IIOService
{
    string ProcessPath {get;}
    IEnumerable<string> EnumerateFiles(string directory);
    bool FileExists(string file);
    string GetAbsolutePath(string path);
}