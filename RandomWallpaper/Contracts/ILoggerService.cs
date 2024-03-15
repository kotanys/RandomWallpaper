namespace RandomWallpaper.Contracts;

public interface ILoggerService
{
    void LogInformation(string message);
    void LogError(string message);
}