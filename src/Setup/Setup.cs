using LurkHelper.Interfaces;
using LurkHelper.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LurkHelper.Setup;

public static class Setup
{
    public static readonly Dictionary<string, string> SwitchMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["--delay"] = nameof(ApplicationConfig.IterationDelay),
        ["--window-swap-delay"] = nameof(ApplicationConfig.WindowSwapDelay),
        ["--app-name"] = nameof(ApplicationConfig.AppName),
        ["--lurks"] = nameof(ApplicationConfig.Lurks),
        ["--streamer"] = nameof(ApplicationConfig.Lurks),
        ["--browser"] = nameof(ApplicationConfig.BrowserExecutable)
    };

    public static IConfiguration BuildConfiguration(string basePath, string[]? args = null)
    {
        args ??= Array.Empty<string>();

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddUserSecrets(typeof(Setup).Assembly, optional: true)
            .AddCommandLine(args, SwitchMappings)
            .Build();
    }

    public static (IServiceCollection, IConfiguration) AddConfig(this IServiceCollection services, string[]? args = null)
    {
        args ??= Array.Empty<string>();
        ValidateArgs(args);

        var config = BuildConfiguration(Directory.GetCurrentDirectory(), args);
        services.AddSingleton<IConfiguration>(config);
        return (services, config);
    }

    public static bool ShouldShowHelp(string[]? args)
    {
        return args?.Any(x => string.Equals(x, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(x, "-h", StringComparison.OrdinalIgnoreCase)) == true;
    }

    public static string BuildHelpText()
    {
        return string.Join(Environment.NewLine, new[]
        {
            "LurkAssistant CLI options:",
            "  --help, -h                 Show this help text",
            "  --streamer <name>         Override streamer/lurk target (maps to Lurks)",
            "  --delay <seconds>         Override iteration delay",
            "  --window-swap-delay <s>   Override delay between window swaps",
            "  --browser <path>          Override browser executable",
            "  --app-name <name>         Override target app/window process name",
            "  --lurks <a,b,c>           Override lurk list with a comma-separated list"
        });
    }

    public static IReadOnlyList<string> GetAppliedArgumentMessages(string[]? args)
    {
        args ??= Array.Empty<string>();
        var messages = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            if (!SwitchMappings.TryGetValue(args[i], out var mappedKey))
            {
                continue;
            }

            if (i + 1 >= args.Length || args[i + 1].StartsWith("-", StringComparison.Ordinal))
            {
                continue;
            }

            messages.Add($"CLI override: {mappedKey} = {args[i + 1]}");
            i++;
        }

        return messages;
    }

    private static void ValidateArgs(IReadOnlyList<string> args)
    {
        for (var i = 0; i < args.Count; i++)
        {
            var key = args[i];
            if (ShouldShowHelp(new[] { key }))
            {
                continue;
            }

            if (!SwitchMappings.ContainsKey(key))
            {
                continue;
            }

            if (i + 1 >= args.Count || args[i + 1].StartsWith("-", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Missing value for {key}.");
            }

            var value = args[i + 1];
            switch (key.ToLowerInvariant())
            {
                case "--delay":
                case "--window-swap-delay":
                    if (!int.TryParse(value, out var delay) || delay <= 0)
                    {
                        throw new ArgumentException($"{key} must be a positive integer.");
                    }
                    break;
                case "--streamer":
                case "--browser":
                case "--app-name":
                case "--lurks":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException($"{key} must have a non-empty value.");
                    }
                    break;
            }

            i++;
        }
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        //services.AddSingleton<MilkdropResidentService>();
        services.AddSingleton<IApplicationConfig, ApplicationConfig>();
        services.AddSingleton<IWindowFinder, WindowFinderAdapter>();
        services.AddSingleton<IBrowserLauncher, ProcessBrowserLauncher>();
        services.AddSingleton<IHostedService, AppSwitcher>();
        //services.AddSingleton<IKeyboardSimulator>(x => new KeyboardSimulator(new InputSimulator()));
        return services;
    }

    public static IServiceCollection AddPolyLogging(this IServiceCollection services, IConfiguration config)
    {
        services.AddLogging(x =>
        {
#if DEBUG
            x.AddDebug();
#endif
            x.AddSeq(config.GetSection("Seq"));
            x.AddConsole();
        });

        return services;
    }
}
