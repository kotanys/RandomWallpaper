using Microsoft.Extensions.DependencyInjection;
using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class AppdataWorker : IAppdataWorker
{
    private readonly IIOService io;
    private readonly string appDataPath;

    public AppdataWorker(IIOService io, 
                        [FromKeyedServices("appdataDirectory")] string appdataDirectory,
                        [FromKeyedServices("Directory.CreateDirectory")] Action<string> createDirectoryAction)
    {
        this.io = io;

        var path = appdataDirectory ?? throw new IOException("Can't find appdata folder");
        appDataPath = Path.Combine(path, "RandomWallpaper");
        createDirectoryAction(appDataPath);
    }

    public FileStream OpenFile(string name, FileMode mode) => io.OpenFile(GetAppdataPath(name), mode);

    public string ReadTextFile(string name)
    {
        using var stream = OpenFile(GetAppdataPath(name), FileMode.Open);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void WriteTextFile(string name, string text)
    {
        using var stream = OpenFile(GetAppdataPath(name), FileMode.Append);
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        writer.WriteLine(text);
    }

    private string GetAppdataPath(string path) => Path.Combine(appDataPath, path);
}