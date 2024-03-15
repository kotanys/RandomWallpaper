using RandomWallpaper.Contracts;
using RandomWallpaper.Models;

namespace RandomWallpaper.Actions;

public class GenerateNowAction(IWallpaperService wallpaper, IRngProvider rng, ILoggerService logger) : IAction<GenerateNowActionArgs>
{
    public void Run(GenerateNowActionArgs args)
    {
        WallpaperModel? chosen = null;
        var random = rng.Generate(0, args.Wallpapers.Select(w => (int)w.Weight).Sum());
        foreach (WallpaperModel wp in args.Wallpapers)
        {
            if (random < wp.Weight)
            {
                chosen = wp;
                break;
            }
            random -= (int)wp.Weight;
        }
        if (chosen is null)
        {
            throw new ArgumentException("Can't choose the wallpaper");
        }
        wallpaper.Set(chosen);
        logger.LogInformation($"Set wallpaper to {chosen.Path}");
    }
}

public class GenerateNowActionArgs(ICollection<WallpaperModel> wallpapers) : EmptyActionArgs
{
    public ICollection<WallpaperModel> Wallpapers { get; } = wallpapers;
}