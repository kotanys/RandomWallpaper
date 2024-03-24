namespace RandomWallpaper.Contracts;

public interface IAppdataWorker
{
    FileStream OpenFile(string name, FileMode mode);
    string ReadTextFile(string name);
    void WriteTextFile(string name, string text);
}