using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class LoggerService(StreamWriter logStream) : ILoggerService, IDisposable
{
    public void LogInformation(string message)
    {
        var toWrite = $"{DateTime.Now:G} INFO: {message}";
        logStream.WriteLine(toWrite);
        logStream.Flush();
    }

    public void LogError(string message)
    {
        var toWrite = $"{DateTime.Now:G} ERROR: {message}";
        logStream.WriteLine(toWrite);
        logStream.Flush();
    }

    public void Dispose()
    {
        logStream.Dispose();
    }
}
