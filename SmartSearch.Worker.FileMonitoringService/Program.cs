using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartSearch.Worker.FileMonitorService;
using SmartSearch.Worker.FileMonitorService.Persistence;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();

        services.AddDbContext<ServiceDbContext>(options => 
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
        },
        ServiceLifetime.Scoped);

        using (var dbContext = (ServiceDbContext)services.BuildServiceProvider().GetRequiredService(typeof(ServiceDbContext)))
        {
            dbContext.Database.Migrate();
        }

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
