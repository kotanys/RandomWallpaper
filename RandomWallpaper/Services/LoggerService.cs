using Microsoft.Extensions.DependencyInjection;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class LoggerService(IAppdataWorker appdataWorker,
                           [FromKeyedServices("Console.WriteLine")] Action<string> consoleWriteLine) : ILoggerService
{
    public void LogInformation(string message)
    {
        var toWrite = $"INFO: {message}";
        appdataWorker.WriteTextFile("log.txt", $"{DateTime.Now:G} {toWrite}");
        consoleWriteLine(toWrite);
    }

    public void LogError(string message)
    {
        var toWrite = $"ERROR: {message}";
        appdataWorker.WriteTextFile("log.txt", $"{DateTime.Now:G} {toWrite}");
        consoleWriteLine(toWrite);
    }
}
