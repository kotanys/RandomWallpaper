namespace RandomWallpaper.Models;

public class Style(int wallpaperStyle, int tileWallpaper)
{
    public int WallpaperStyle { get; } = wallpaperStyle;
    public int TileWallpaper { get; } = tileWallpaper;

    public static readonly Style Fill = new(10, 0);
    public static readonly Style Fit = new(6, 0);
    public static readonly Style Stretch = new(2, 0);
    public static readonly Style Tile = new(0, 1);
    public static readonly Style Center = new(0, 0);
    public static readonly Style Span = new(22, 0);
}