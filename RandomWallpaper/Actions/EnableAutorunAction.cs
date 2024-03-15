using RandomWallpaper.Contracts;

namespace RandomWallpaper.Actions;

public class EnableAutorunAction(IAutorunService autorun) : IAction<EnableAutorunActionArgs>
{
    public void Run(EnableAutorunActionArgs args)
    {
        if (args.WpcfgPath[0] == '"' && args.WpcfgPath[^1] == '"')
            autorun.Add($"now {args.WpcfgPath}");
        else
            autorun.Add($"now \"{args.WpcfgPath}\"");
    }
}

public class EnableAutorunActionArgs(string wpcfgPath) : EmptyActionArgs
{
    public string WpcfgPath { get; } = wpcfgPath;
}