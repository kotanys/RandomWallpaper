using Moq;
using RandomWallpaper.Contracts;
using RandomWallpaper.Models;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class WpcfgParserServiceTests
{
    private readonly WpcfgParserService parser;

    public WpcfgParserServiceTests()
    {
        var mockIO = Mock.Of<IIOService>(
            io => io.FileExists(It.IsAny<string>()) == true &&
                io.EnumerateFiles(It.IsAny<string>()) == Enumerable.Empty<string>()
        );
        Mock.Get(mockIO)
            .Setup(io => io.GetAbsolutePath(It.IsAny<string>()))
            .Returns<string>(path => $"c:\\{path}");

        parser = new WpcfgParserService(mockIO);
    }

    [Theory]
    [InlineData("image.png Fit 10", "image.png")]
    [InlineData(" image.jpeg  fit   10", "image.jpeg")]
    public void ShouldReturnCorrentWallpaperModels(string valid, string fileName)
    {
        var model = parser.ParseOne(valid);
        var correct = new WallpaperModel($"c:\\{fileName}", Style.Fit, 10);
        Assert.Equal(correct, model);
    }

    [Theory]
    [InlineData("not_image.txt fill 5")]
    [InlineData("aint_no_ext grid 10")]
    public void ShouldThrowOnInvalidFileExt(string invalid)
    {
        Action action = () => parser.ParseOne(invalid);
        Assert.Throws<ArgumentException>(action);
    }
    
    [Fact]
    public void ShouldThrowOnNotClosedBraces()
    {
        Action action = () => parser.ParseOne("\"blah blah blah.txt fit 10");
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void WorksWithQuotes()
    {
        var model = parser.ParseOne("\"This is a file with spaces.png\" fit 10");
        var correct = new WallpaperModel("c:\\This is a file with spaces.png", Style.Fit, 10);
        Assert.Equal(correct, model);
    }
}