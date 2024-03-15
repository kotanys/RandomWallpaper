using RandomWallpaper.Models;

namespace RandomWallpaper.Contracts;

public interface IWpcfgParserService
{
    ICollection<WallpaperModel> ParseFile(string wpcfg);
    public WallpaperModel ParseOne(string line);
}