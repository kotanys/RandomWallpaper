using Moq;
using RandomWallpaper.Contracts;
using RandomWallpaper.Services;

namespace RandomWallpaper.Tests;

public class WpcfgFinderServiceTests
{
    private const string RegAutorunKey = "RandomWallpaper Autorun TEST";
    private const string AutorunRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private readonly Mock<IIOService> ioMock;
    private readonly Mock<IRegistryService> registryMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly WpcfgFinderService finder;

    public WpcfgFinderServiceTests()
    {
        ioMock = new Mock<IIOService>();
        ioMock.Setup(io => io.ProcessPath).Returns(@"c:\Appdir\app.exe");
        registryMock = new Mock<IRegistryService>();
        loggerMock = new Mock<ILoggerService>();

        finder = new WpcfgFinderService(ioMock.Object, registryMock.Object, loggerMock.Object, RegAutorunKey);
    }

    [Fact]
    public void ShouldFindInRegistry()
    {
        const string expectedWpcfg = @"c:\yay_you_found_me.wpcfg";
        object? registryValue = @"c:\Appdir\app.exe now " + expectedWpcfg;
        registryMock.Setup(r => r.TryGetValue(AutorunRegPath, RegAutorunKey, out registryValue)).Returns(true);

        string actual = finder.Find();

        registryMock.Verify();
        Assert.Equal(expectedWpcfg, actual);
    } 

    [Fact]
    public void ShouldFindInAppdir()
    {
        object? nil = null;
        const string expectedWpcfg = @"c:\Appdir\found.wpcfg";
        ioMock.Setup(io => io.EnumerateFiles(@"c:\Appdir\")).Returns<string>(_ => ["wrong.txt", "app.exe", "found.wpcfg"]);
        registryMock.Setup(r => r.TryGetValue(AutorunRegPath, RegAutorunKey, out nil));

        string actual = finder.Find();

        registryMock.Verify();
        ioMock.Verify();
        Assert.Equal(expectedWpcfg, actual);
    }

    [Fact]
    public void ShouldThrowOnNotFound()
    {
        object? nil = null;
        ioMock.Setup(io => io.EnumerateFiles(@"c:\Appdir\")).Returns<string>(_ => ["wrong.txt", "app.exe"]);
        registryMock.Setup(r => r.TryGetValue(AutorunRegPath, RegAutorunKey, out nil));
        loggerMock.Setup(lg => lg.LogError(It.IsAny<string>()));

        Action shouldThrow = () => finder.Find();

        Assert.Throws<IOException>(shouldThrow);
        registryMock.Verify();
        ioMock.Verify();
        loggerMock.Verify(lg => lg.LogError(It.IsAny<string>()), Times.AtLeastOnce());
    }

    [Fact]
    public void ShouldNotReturnQuotesIfPathHasSpaces_Registry()
    {
        const string expectedWpcfg = @"c:\yay you found me.wpcfg";
        object? registryValue = @"c:\Appdir\app.exe now " + expectedWpcfg;
        registryMock.Setup(r => r.TryGetValue(AutorunRegPath, RegAutorunKey, out registryValue)).Returns(true);

        string actual = finder.Find();

        registryMock.Verify();
        Assert.Equal(expectedWpcfg, actual);
    }

    [Fact]
    public void ShouldNotReturnQuotesIfPathHasSpaces_Appdir()
    {
        object? nil = null;
        const string expectedWpcfg = @"c:\Appdir\found space .wpcfg";
        ioMock.Setup(io => io.EnumerateFiles(@"c:\Appdir\")).Returns<string>(_ => ["found space .wpcfg"]);
        registryMock.Setup(r => r.TryGetValue(AutorunRegPath, RegAutorunKey, out nil));

        string actual = finder.Find();

        registryMock.Verify();
        ioMock.Verify();
        Assert.Equal(expectedWpcfg, actual);
    }
}