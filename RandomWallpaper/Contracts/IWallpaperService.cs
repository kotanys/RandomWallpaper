using RandomWallpaper.Models;

namespace RandomWallpaper.Contracts;

public interface IWallpaperService
{
    void Set(WallpaperModel wallpaper);
}