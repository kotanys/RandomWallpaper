using Microsoft.Extensions.DependencyInjection;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class WpcfgFinderService(IIOService io, IRegistryService registry, ILoggerService logger,
                                [FromKeyedServices("reg autorun key")] string regAutorunKey) : IWpcfgFinderService
{
    public string Find()
    {
        logger.LogInformation("Finder is trying to locate .wpcfg");

        if (registry.TryGetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", regAutorunKey, out object? value) && value is not null)
        {
            var path = ExtractWpcfgPath((string)value);
            logger.LogInformation($"Found in registry: {path}");
            return path;
        }

        string appDirectory = new(io.ProcessPath.Reverse().SkipWhile(p => p != '\\').Reverse().ToArray());
        var wpcfgInAppDirectory = io.EnumerateFiles(appDirectory).FirstOrDefault(f => Path.GetExtension(f) == ".wpcfg");
        if (wpcfgInAppDirectory is not null)
        {
            wpcfgInAppDirectory = Path.Combine(appDirectory, wpcfgInAppDirectory);
            logger.LogInformation($"Found in application directory: {wpcfgInAppDirectory}");
            return wpcfgInAppDirectory;
        }

        logger.LogError("Couldn't locate .wpcfg");
        throw new IOException("Couldn't locate .wpcfg file");
    }

    private static string ExtractWpcfgPath(string registryValue)
    {
        var split = registryValue.Split(' ').SkipWhile(s => s != "now").Skip(1);
        var path = string.Join(' ', split);
        if (path[0] == '"' && path[^1] == '"')
        {
            path = new string(path.Skip(1).SkipLast(1).ToArray());
        }
        return path;
    }
}