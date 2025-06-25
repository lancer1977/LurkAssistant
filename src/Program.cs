// See https://aka.ms/new-console-template for more information
using LurkHelper.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PolyhydraGames.Core.Console.System;


Console.WriteLine("Started");
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var host = Host.CreateDefaultBuilder(args);
IConfiguration config;
var app = host.ConfigureServices((services) =>
{

    config = services.AddConfig().Item2;
    services.AddServices();
    services.AddPolyLogging(config);


}).Build();
await app.StartAsync();

