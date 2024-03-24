using RandomWallpaper.Actions;
using RandomWallpaper.Contracts;
using RandomWallpaper.Models;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class GenerateNowActionTests
{
    private readonly MockWallpaperService mockWallpaperService = new();
    private readonly MockLogger mockLogger = new();
    private readonly GenerateNowAction generateNowAction;

    [Fact]
    public void ShouldSetCorrentWallpaper()
    {
        var mostLikely = new WallpaperModel("weight5", Style.Fit, 5);
        var lessLikely = new WallpaperModel("weight2", Style.Fit, 2);
        var unlikely = new WallpaperModel("weight1", Style.Fit, 1);
        var never = new WallpaperModel("weight0", Style.Fit, 0);
        WallpaperModel[] wallpapers = [ mostLikely, lessLikely, unlikely, never ];
        var args = new GenerateNowActionArgs(wallpapers);

        var results = wallpapers.Select(w => (w, 0)).ToDictionary();
        for (int i = 0; i < 4000; i++)
        {
            generateNowAction.Run(args);
            if (mockWallpaperService.SetWallpaper is null)
                Assert.Fail("Wallpaper wasn't chosen at all");
            results[mockWallpaperService.SetWallpaper]++;
        }

        Assert.True(results[mostLikely] > results[lessLikely]);
        Assert.True(results[lessLikely] > results[unlikely]);
        Assert.True(results[never] == 0);
    }

    [Fact]
    public void ShouldThrowOnNoWeight()
    {
        WallpaperModel[] noWeight = [ new WallpaperModel("nope", Style.Fit, 0) ];
        Action shouldThrow = () => generateNowAction.Run(new(noWeight));
        Assert.Throws<ArgumentException>(shouldThrow);
    }
    
    public GenerateNowActionTests()
    {
        generateNowAction = new(mockWallpaperService, new RngProvider(), mockLogger);
    }

    private class MockWallpaperService : IWallpaperService
    {
        public WallpaperModel? SetWallpaper { get; private set; }

        public void Set(WallpaperModel wallpaper)
        {
            SetWallpaper = wallpaper;
        }
    }

    private class MockLogger : ILoggerService
    {
        public int CalledLogInformationTimes {get; private set;}
        public int CalledLogErrorTimes {get; private set;}

        public void LogError(string message) => CalledLogErrorTimes++;

        public void LogInformation(string message) => CalledLogInformationTimes++;
    }
}