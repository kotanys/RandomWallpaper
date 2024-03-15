using System.Text;
using RandomWallpaper.Contracts;
using RandomWallpaper.Models;

namespace RandomWallpaper.Services;

public class WpcfgParserService(IIOService io) : IWpcfgParserService
{
    private const char Separator = ' ';

    public ICollection<WallpaperModel> ParseFile(string wpcfg)
    {
        string dir = new(wpcfg.Reverse().SkipWhile(p => p != '\\').Reverse().ToArray());
        return File.ReadAllLines(wpcfg)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => {
                        if (l.Contains(':'))
                            return ParseOne(l);
                        else
                            return ParseOneWithWpcfgPath(l, dir);
                    }).ToList();
    }

    public WallpaperModel ParseOne(string line)
    {
        return ParseOneWithWpcfgPath(line, "");
    }

    private WallpaperModel ParseOneWithWpcfgPath(string line, string wpcfgLocation)
    {
        line = line.Trim();
        var path = ExtractPath(line, wpcfgLocation, out string styleAndWeight);
        var style = ExtractStyle(styleAndWeight, out string weightText);
        if (!uint.TryParse(weightText, out uint weight))
        {
            throw new ArgumentException($"Weight {weightText} must be a number");
        }
        var wallpaper = new WallpaperModel(path, style, weight);
        return wallpaper;
    }

    private string ExtractPath(string line, string wpcfgLocation, out string trailing)
    {
        var pathBuilder = new StringBuilder(capacity: line.Length);
        char stopAt = line[0] == '"' ? '"' : Separator;
        int i = stopAt == '"' ? 1 : 0;
        while (i < line.Length && line[i] != stopAt)
        {
            pathBuilder.Append(line[i]);
            i++;
        }
        if (i == line.Length)
        {
            throw new ArgumentException($"Can't parse this: {line}");
        }

        string path;
        if (string.IsNullOrEmpty(wpcfgLocation))
            path = io.GetAbsolutePath(pathBuilder.ToString());
        else
            path = Path.Combine(wpcfgLocation, pathBuilder.ToString());
        
        if (!io.FileExists(path) || !CheckExtension(pathBuilder.ToString()))
        {
            throw new ArgumentException($"File {path} doesn't exist or it's not a valid image.");
        }
        trailing = line[(i + 1)..].Trim();
        return path;
    }

    private Style ExtractStyle(string from, out string weightText)
    {
        var builder = new StringBuilder();
        int i = 0; 
        while (i < from.Length && from[i] != Separator)
        {
            builder.Append(char.ToLower(from[i]));
            i++;
        }
        weightText = from[i..].Trim();
        return builder.ToString() switch 
        {
            "fill" => Style.Fill,
            "fit" => Style.Fit,
            "stretch" => Style.Stretch,
            "tile" => Style.Tile,
            "center" => Style.Center,
            "span" => Style.Span,
            _ => throw new ArgumentException($"There is no {builder} style")
        };
    }

    private static bool CheckExtension(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        return ext switch
        {
            ".jpeg" or ".jpg" or ".bmp" or ".png" => true,
            _ => false,
        };
    }

}
