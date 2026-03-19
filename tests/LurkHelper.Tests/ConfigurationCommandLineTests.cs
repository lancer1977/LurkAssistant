using LurkHelper.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace LurkHelper.Tests;

public class ConfigurationCommandLineTests
{
    [Fact]
    public void BuildConfiguration_WithEmptyArgs_LoadsJsonSettings()
    {
        var tempDir = CreateTempConfigDir();

        try
        {
            var config = global::LurkHelper.Setup.Setup.BuildConfiguration(tempDir, Array.Empty<string>());

            Assert.Equal("30", config[nameof(ApplicationConfig.IterationDelay)]);
            Assert.Equal("msedge", config[nameof(ApplicationConfig.AppName)]);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void BuildConfiguration_WithCommandLineArgs_OverridesJsonSettings()
    {
        var tempDir = CreateTempConfigDir();

        try
        {
            var args = new[]
            {
                "--delay", "12",
                "--app-name", "firefox",
                "--browser", "/usr/bin/firefox"
            };

            var config = global::LurkHelper.Setup.Setup.BuildConfiguration(tempDir, args);

            Assert.Equal("12", config[nameof(ApplicationConfig.IterationDelay)]);
            Assert.Equal("firefox", config[nameof(ApplicationConfig.AppName)]);
            Assert.Equal("/usr/bin/firefox", config[nameof(ApplicationConfig.BrowserExecutable)]);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void BuildConfiguration_WithStreamerArg_OverridesLurksSetting()
    {
        var tempDir = CreateTempConfigDir();

        try
        {
            var config = global::LurkHelper.Setup.Setup.BuildConfiguration(tempDir, ["--streamer", "PolyhydraGames"]);

            Assert.Equal("PolyhydraGames", config[nameof(ApplicationConfig.Lurks)]);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void ValidateArgs_WithInvalidDelay_ThrowsHelpfulError()
    {
        var ex = Assert.Throws<ArgumentException>(() => new ServiceCollection().AddConfig(["--delay", "0"]));

        Assert.Equal("--delay must be a positive integer.", ex.Message);
    }

    [Fact]
    public void BuildHelpText_IncludesRequestedOptions()
    {
        var helpText = global::LurkHelper.Setup.Setup.BuildHelpText();

        Assert.Contains("--help", helpText);
        Assert.Contains("--streamer <name>", helpText);
        Assert.Contains("--delay <seconds>", helpText);
        Assert.Contains("--browser <path>", helpText);
    }

    [Fact]
    public void ShouldShowHelp_DetectsLongHelpFlag()
    {
        Assert.True(global::LurkHelper.Setup.Setup.ShouldShowHelp(["--help"]));
    }

    [Fact]
    public void ShouldShowHelp_DetectsShortHelpFlag()
    {
        Assert.True(global::LurkHelper.Setup.Setup.ShouldShowHelp(["-h"]));
    }

    [Fact]
    public void GetAppliedArgumentMessages_ReturnsConfiguredOverrides()
    {
        var messages = global::LurkHelper.Setup.Setup.GetAppliedArgumentMessages(["--streamer", "PolyhydraGames", "--delay", "15", "--browser", "/usr/bin/firefox"]);

        Assert.Contains("CLI override: Lurks = PolyhydraGames", messages);
        Assert.Contains("CLI override: IterationDelay = 15", messages);
        Assert.Contains("CLI override: BrowserExecutable = /usr/bin/firefox", messages);
    }

    private static string CreateTempConfigDir()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"lurkassistant-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var json = """
        {
          "BrowserExecutable": "msedge.exe",
          "AppName": "msedge",
          "WindowSwapDelay": "5",
          "IterationDelay": "30",
          "Lurks": "DreadBreadcrumb"
        }
        """;

        File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);
        return tempDir;
    }
}
