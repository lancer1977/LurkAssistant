using LurkHelper.Interfaces;
using LurkHelper.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using AppWindow = PolyhydraGames.Core.Console.System.AppWindow;

namespace LurkHelper.Tests;

public class AppSwitcherTests
{
    [Fact]
    public void OpenLurks_OpensOnlyMissingWindows()
    {
        var config = CreateConfig(lurks: ["existing", "missing"]);
        var windows = new List<AppWindow>
        {
            new(1, "existing - Twitch", IntPtr.Zero)
        };

        var windowFinder = new Mock<IWindowFinder>();
        windowFinder.Setup(x => x.GetWindows(config.Object.AppName)).Returns(windows);

        var browserLauncher = new Mock<IBrowserLauncher>();
        var sut = CreateSubject(config, windowFinder, browserLauncher);

        sut.OpenLurks();

        browserLauncher.Verify(
            x => x.OpenNewWindow(config.Object.BrowserExecutable, "https://twitch.tv/missing"),
            Times.Once);
        browserLauncher.Verify(
            x => x.OpenNewWindow(config.Object.BrowserExecutable, "https://twitch.tv/existing"),
            Times.Never);
    }

    [Fact]
    public void OpenLurks_WhenAllWindowsExist_DoesNotOpenBrowser()
    {
        var config = CreateConfig(lurks: ["dreadbreadcrumb"]);
        var windows = new List<AppWindow>
        {
            new(1, "DreadBreadcrumb is live", IntPtr.Zero)
        };

        var windowFinder = new Mock<IWindowFinder>();
        windowFinder.Setup(x => x.GetWindows(config.Object.AppName)).Returns(windows);

        var browserLauncher = new Mock<IBrowserLauncher>();
        var sut = CreateSubject(config, windowFinder, browserLauncher);

        sut.OpenLurks();

        browserLauncher.Verify(
            x => x.OpenNewWindow(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void SwitchWindows_SwitchesToEachDiscoveredWindow()
    {
        var config = CreateConfig();
        var windows = new List<AppWindow>
        {
            new(1, "First", (IntPtr)1),
            new(2, "Second", (IntPtr)2),
            new(3, "Third", (IntPtr)3)
        };

        var windowFinder = new Mock<IWindowFinder>();
        windowFinder.Setup(x => x.GetWindows(config.Object.AppName)).Returns(windows);

        var browserLauncher = new Mock<IBrowserLauncher>();
        var sut = CreateSubject(config, windowFinder, browserLauncher);

        sut.SwitchWindows();

        foreach (var window in windows)
        {
            windowFinder.Verify(x => x.SwitchToWindow(window), Times.Once);
        }
    }

    private static AppSwitcher CreateSubject(
        Mock<IApplicationConfig> config,
        Mock<IWindowFinder> windowFinder,
        Mock<IBrowserLauncher> browserLauncher)
    {
        return new AppSwitcher(
            config.Object,
            NullLogger<AppSwitcher>.Instance,
            windowFinder.Object,
            browserLauncher.Object);
    }

    private static Mock<IApplicationConfig> CreateConfig(
        int iterationDelay = 1,
        int windowSwapDelay = 1,
        string appName = "msedge",
        List<string>? lurks = null,
        string browserExecutable = "msedge")
    {
        var config = new Mock<IApplicationConfig>();
        config.SetupGet(x => x.IterationDelay).Returns(iterationDelay);
        config.SetupGet(x => x.WindowSwapDelay).Returns(windowSwapDelay);
        config.SetupGet(x => x.AppName).Returns(appName);
        config.SetupGet(x => x.Lurks).Returns(lurks ?? []);
        config.SetupGet(x => x.BrowserExecutable).Returns(browserExecutable);
        return config;
    }
}
