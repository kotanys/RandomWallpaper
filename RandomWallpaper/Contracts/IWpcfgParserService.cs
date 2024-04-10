using RandomWallpaper.Models;

namespace RandomWallpaper.Contracts;

public interface IWpcfgParserService
{
    ICollection<WallpaperModel> Parse(string wpcfg, string wpcfgPath);
    public WallpaperModel ParseOne(string line);
}