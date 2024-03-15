using RandomWallpaper.Contracts;
namespace RandomWallpaper.Actions;

public class DisableAutorunAction(IAutorunService autorun) : IAction<EmptyActionArgs>
{
    public void Run(EmptyActionArgs args) => autorun.Remove();
}
