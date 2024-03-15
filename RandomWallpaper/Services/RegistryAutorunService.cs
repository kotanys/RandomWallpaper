using Microsoft.Extensions.DependencyInjection;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class RegistryAutorunService(IIOService io, IRegistryService registry, ILoggerService logger,
                                    [FromKeyedServices("reg autorun key")] string regKey) : IAutorunService
{
    private const string AutorunSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public void Add(string arguments)
    {
        var appPath = Path.GetFullPath(io.ProcessPath ?? throw new InvalidOperationException("Can't get path of this app"));
        registry.SetValue(AutorunSubKey, regKey, appPath + " " + arguments);
        logger.LogInformation($"Added app to autorun");
    }

    public void Remove()
    {
        if (!registry.TryGetValue(AutorunSubKey, regKey, out _))
        {
            logger.LogError($"Can't remove app from autorun");
            logger.LogInformation("Maybe it was never added in the first place");
            return;
        }
        registry.DeleteValue(AutorunSubKey, regKey, false);
    }
}
