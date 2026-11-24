using CityCar.Worker;
using DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository;
using Service.Services;
using Microsoft.Extensions.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() 

    .UseContentRoot(AppContext.BaseDirectory)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<CityCarDb>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IContext, CityCarDb>();

      
        services.AddMemoryCache();

        services.AddServices(hostContext.Configuration);

        services.AddTransient<OrderTrackingService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();