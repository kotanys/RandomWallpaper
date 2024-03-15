using RandomWallpaper.Actions;

namespace RandomWallpaper.Contracts;

public interface IAction<T> : IAction where T : EmptyActionArgs
{
    void Run(T args);
}


public interface IAction {}