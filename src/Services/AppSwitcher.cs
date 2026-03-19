using LurkHelper.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Core.Console.System;

namespace LurkHelper.Services;
public class AppSwitcher : IHostedService
{
    private readonly IApplicationConfig _config;
    private readonly ILogger<AppSwitcher> _logger;
    private readonly IWindowFinder _windowFinder;
    private readonly IBrowserLauncher _browserLauncher;

    public AppSwitcher(
        IApplicationConfig config,
        ILogger<AppSwitcher> logger,
        IWindowFinder windowFinder,
        IBrowserLauncher browserLauncher)
    {
        _config = config;
        _logger = logger;
        _windowFinder = windowFinder;
        _browserLauncher = browserLauncher;

        _logger.LogInformation($"AppSwitcher Version: ");
        _logger.LogInformation($"Process Name Target: {config.AppName}");
        _logger.LogInformation($"Seconds Between Window Switches: {config.WindowSwapDelay}");
        _logger.LogInformation($"Seconds Between Iterations: {config.IterationDelay}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        OpenLurks();
        await SwitchWindowProcess(cancellationToken);
    }

    private async Task SwitchWindowProcess(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            SwitchWindows();
            _logger.LogInformation($"Sleeping for {_config.IterationDelay} seconds.");
            await Task.Delay(TimeSpan.FromSeconds(_config.IterationDelay), cancellationToken);
        }
    }

    public void SwitchWindows()
    {
        var windows = _windowFinder.GetWindows(_config.AppName);
        foreach (var window in windows)
        {
            _windowFinder.SwitchToWindow(window);
            Task.Delay(_config.IterationDelay * 1000);
        }
    }

    public void OpenLurks()
    {
        foreach (var item in _config.Lurks)
        {
            if (WindowWithTitleExists(item))
            {
                continue;
            }

            var url = $"https://twitch.tv/{item}";
            _browserLauncher.OpenNewWindow(_config.BrowserExecutable, url);
        }
    }

    private bool WindowWithTitleExists(string item)
    {
        var windows = _windowFinder.GetWindows(_config.AppName);
        var existing = windows.Where(x => x.Title.Contains(item, StringComparison.OrdinalIgnoreCase));
        return existing.Any();
    }
}
