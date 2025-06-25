using System.Diagnostics;
using LurkHelper.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Core.Console.System;

namespace LurkHelper.Services;
public class AppSwitcher : IHostedService
{
    private readonly IApplicationConfig _config;
    private readonly ILogger<AppSwitcher> _logger;

    public AppSwitcher(IApplicationConfig config, ILogger<AppSwitcher> logger)
    {
        _config = config;
        _logger = logger;

        _logger.LogInformation($"AppSwitcher Version: ");
        _logger.LogInformation($"Process Name Target: {config.AppName}");
        _logger.LogInformation($"Seconds Between Window Switches: {config.WindowSwapDelay}");
        _logger.LogInformation($"Seconds Between Itterations: {config.ItterationDelay}");
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
            _logger.LogInformation($"Sleeping for {_config.ItterationDelay} seconds.");
            await Task.Delay(TimeSpan.FromSeconds(_config.ItterationDelay), cancellationToken);

        }
    }



    private void SwitchWindows()
    {
        var windows = WindowFinder.GetWindows(_config.AppName);
        foreach (var window in windows)
        {
            WindowFinder.SwitchToWindow(window);
            Task.Delay(_config.ItterationDelay * 1000);
        }
        
    }



    public void OpenLurks()
    {
        foreach (var item in _config.Lurks)
        {
            if (WindowWithTitleExists(item)) continue;
            var url = $"https://twitch.tv/{item}";
            OpenNewUrl(url);
        }
    }
    private bool WindowWithTitleExists(string item)
    {
        var windows = WindowFinder.GetWindows(_config.AppName);
        var existing = windows.Where(x => x.Title.Contains(item, StringComparison.OrdinalIgnoreCase));
        return existing.Any();
    }
    private void OpenNewUrl(string url)
    {
        Process.Start(_config.BrowserExecutable, "--new-window " + url);
    }
}