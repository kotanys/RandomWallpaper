using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class WpcfgFinderService(IIOService io, IRegistryService registry, ILoggerService logger) : IWpcfgFinderService
{
    public string Find()
    {
        logger.LogInformation("Finder is trying to locate .wpcfg");

        if (registry.TryGetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "RandomWallpaper Autorun", out object? value) && value is not null)
        {
            var path = ExtractWpcfgPath((string)value);
            logger.LogInformation($"Found in registry: {path}");
            return path;
        }

        string dir = new(io.ProcessPath.Reverse().SkipWhile(p => p != '\\').Reverse().ToArray());
        var inDir = io.EnumerateFiles(dir).FirstOrDefault(f => Path.GetExtension(f) == ".wpcfg");
        if (inDir is not null)
        {
            logger.LogInformation($"Found in application directory: {inDir}");
            return inDir;
        }

        logger.LogError("Couldn't locate .wpcfg");
        throw new InvalidOperationException("Couldn't locate .wpcfg file");
    }

    private static string ExtractWpcfgPath(string registryValue)
    {
        var split = registryValue.Split(' ').SkipWhile(s => s != "now").Skip(1);
        return string.Join(' ', split);
    }
}