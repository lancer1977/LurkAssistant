using LurkHelper.Interfaces;
using LurkHelper.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LurkHelper.Setup;

public static class Setup
{
    public static (IServiceCollection, IConfiguration) AddConfig(this IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the test project
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("165e6907-cc47-4f3f-b643-07bb0ee58c03") // Use the UserSecretsId generated earlier
            .Build();
        services.AddSingleton<IConfiguration>(config);
        return (services, config);
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        //services.AddSingleton<MilkdropResidentService>();
        services.AddSingleton<IApplicationConfig, ApplicationConfig>();
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