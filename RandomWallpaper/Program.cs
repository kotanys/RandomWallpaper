using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using RandomWallpaper.Actions;
using RandomWallpaper.Contracts;
using RandomWallpaper.Services;

[assembly: SupportedOSPlatform("windows")]
[assembly: InternalsVisibleTo("RandomWallpaper.Tests")]

#if DEBUG
args = ["now"];
#endif

const string RegAutorunKey = "RandomWallpaper Autorun";

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var services = new ServiceCollection();
services.AddKeyedSingleton("reg autorun key", RegAutorunKey);
services.AddSingleton<IWallpaperService, WallpaperService>();
services.AddSingleton<IRegistryService, RegistryService>();
services.AddSingleton<IAutorunService, RegistryAutorunService>();
services.AddSingleton<IIOService, IOService>();
services.AddSingleton<IRngProvider, RngProvider>();
services.AddSingleton<IWpcfgParserService, WpcfgParserService>();
services.AddSingleton<IWpcfgFinderService, WpcfgFinderService>();
SetupLogging();
switch (args[0].ToLower())
{
    case "enable":
        services.AddSingleton<IAction, EnableAutorunAction>();
        break;
    case "disable":
        services.AddSingleton<IAction, DisableAutorunAction>();
        break;
    case "now":
        services.AddSingleton<IAction, GenerateNowAction>();
        break;
    default:
        PrintUsage($"There is no {args[0]} action.");
        return;
}

var provider = services.BuildServiceProvider();
try
{
    RunAction(provider);
}
catch (Exception e)
{
    var logger = provider.GetService<ILoggerService>();
    logger?.LogError($"Fatal error: {e.Message}");
}

void RunAction(IServiceProvider services)
{
    var action = services.GetRequiredService<IAction>();
    if (action is EnableAutorunAction enableAction)
    {
        var args = new EnableAutorunActionArgs(GetWpcfgPath());
        enableAction.Run(args);
    }
    else if (action is DisableAutorunAction disableAction)
    {
        disableAction.Run(EmptyActionArgs.Empty);
    }
    else if (action is GenerateNowAction generateNowAction)
    {
        string wpcfg = GetWpcfgPath();
        var wallpapers = services.GetRequiredService<IWpcfgParserService>().ParseFile(wpcfg);
        generateNowAction.Run(new(wallpapers));
    }
}

static void PrintUsage(string? additionalInfo = null)
{
    string thisAppName = Path.GetFileName(Environment.ProcessPath!);
    Console.WriteLine("Usage:");
    Console.WriteLine($"\t{thisAppName} enable|disable|now [.wpcfg file]");
    Console.WriteLine();
    Console.WriteLine("if .wpcfg file is not provided explicitly, there will be an attempt to find it.");
    Console.WriteLine();
    if (additionalInfo is not null)
        Console.WriteLine(additionalInfo);
}

string GetWpcfgPath()
{
    string dir = new(Environment.ProcessPath!.Reverse().SkipWhile(p => p != '\\').Reverse().ToArray());
    if (args.Length > 1)
    {
        return Path.Combine(dir, args[1]);
    }

    var finder = provider.GetService<IWpcfgFinderService>();
    if (finder is not null)
    {
        try
        {
            var found = finder.Find();
            if (!found.Contains(':'))
                return Path.Combine(dir, found);
            else return found;
        }
        catch {}
    }

    throw new ArgumentException("Can't get .wpcfg file");
}

void SetupLogging()
{
    var path = Path.Combine(Environment.ProcessPath!, "log.txt");
    services.AddSingleton<ILoggerService>(new LoggerService(new StreamWriter(path, true)));
}