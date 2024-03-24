using Moq;
using RandomWallpaper.Actions;
using RandomWallpaper.Contracts;
using RandomWallpaper.Models;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class GenerateNowActionTests
{
    private readonly Mock<IWallpaperService> wallpaperServiceMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GenerateNowAction generateNowAction;

    
    public GenerateNowActionTests()
    {
        wallpaperServiceMock = new Mock<IWallpaperService>();
        loggerMock = new Mock<ILoggerService>();
        loggerMock.Setup(lg => lg.LogInformation(It.IsAny<string>()));
        loggerMock.Setup(lg => lg.LogError(It.IsAny<string>()));
        generateNowAction = new(wallpaperServiceMock.Object, new RngProvider(), loggerMock.Object);
    }

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
        wallpaperServiceMock
            .Setup(wp => wp.Set(It.IsAny<WallpaperModel>()))
            .Callback<WallpaperModel>(chosen => results[chosen]++);

        const int RunTimes = 4000;
        for (int i = 0; i < RunTimes; i++)
        {
            generateNowAction.Run(args);
        }

        Assert.True(results.Values.Sum() == RunTimes);
        Assert.True(results[mostLikely] > results[lessLikely]);
        Assert.True(results[lessLikely] > results[unlikely]);
        Assert.True(results[never] == 0);
        loggerMock.Verify(
            lg => lg.LogError(It.IsAny<string>()), Times.Never()
        );
        loggerMock.Verify(
            lg => lg.LogInformation(It.IsAny<string>()), Times.AtLeast(RunTimes)
        );
    }

    [Fact]
    public void ShouldThrowOnNoWeight()
    {
        WallpaperModel[] noWeight = [ new WallpaperModel("nope", Style.Fit, 0) ];
        Action shouldThrow = () => generateNowAction.Run(new(noWeight));
        Assert.Throws<ArgumentException>(shouldThrow);
        loggerMock.Verify(
            lg => lg.LogError(It.IsAny<string>()), Times.AtLeastOnce()
        );
    }
}