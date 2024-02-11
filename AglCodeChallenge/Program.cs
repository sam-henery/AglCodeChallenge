using AglCodeChallenge;
using AglCodeChallenge.ApiServices;
using AglCodeChallenge.Options;
using AglCodeChallenge.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = CreateHostBuilder(args).Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

static IHostBuilder CreateHostBuilder(string[] strings)
{
    var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(builder =>
        {
            builder.AddConfiguration(configuration);
        })
        .ConfigureServices((_, services) =>
        {
            services.Configure<EndpointsOptions>(configuration.GetSection("Endpoints"));
            services.Configure<SettingsOptions>(configuration.GetSection("Settings"));
            services.AddOptions();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });
            services.AddSingleton<IPetApiService, PetApiService>();
            services.AddSingleton<IPetService, PetService>();
            services.AddSingleton<App>();            
        });
}
