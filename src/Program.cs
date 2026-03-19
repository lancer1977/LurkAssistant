// See https://aka.ms/new-console-template for more information
using LurkHelper.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Core.Console.System;

if (Setup.ShouldShowHelp(args))
{
    Console.WriteLine(Setup.BuildHelpText());
    return;
}

Console.WriteLine("Started");
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var host = Host.CreateDefaultBuilder(args);
IConfiguration config;
var app = host.ConfigureServices((services) =>
{
    config = services.AddConfig(args).Item2;
    services.AddServices();
    services.AddPolyLogging(config);
}).Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
foreach (var message in Setup.GetAppliedArgumentMessages(args))
{
    logger.LogInformation(message);
}

await app.StartAsync();
