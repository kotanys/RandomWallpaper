using System.Runtime.InteropServices;
using Microsoft.Win32;
using RandomWallpaper.Contracts;
using RandomWallpaper.Models;

namespace RandomWallpaper.Services;

public class WallpaperService(IRegistryService registryService) : IWallpaperService
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    private const int SPI_SETDESKWALLPAPER = 0x14;
    private const int SPIF_UPDATEINIFILE = 0x1;
    private const int SPIF_SENDCHANGE = 0x2;


    public void Set(WallpaperModel wallpaper)
    {
        if (!Path.Exists(wallpaper.Path))
        {
            throw new FileNotFoundException($"{wallpaper.Path} does not exist!");
        }
        SetStyle(wallpaper.Style);
        bool success = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaper.Path, SPIF_SENDCHANGE | SPIF_UPDATEINIFILE);
        if (!success)
        {
            throw new InvalidOperationException("The wallpaper could not be set.");
        }
    }

    private void SetStyle(Style style)
    {
        registryService.SetValue(@"Control Panel\Desktop", "WallpaperStyle", style.WallpaperStyle);
        registryService.SetValue(@"Control Panel\Desktop", "TileWallpaper", style.TileWallpaper);
    }
}
