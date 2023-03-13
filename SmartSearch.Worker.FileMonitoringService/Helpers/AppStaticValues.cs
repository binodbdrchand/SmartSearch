using Microsoft.EntityFrameworkCore;
using SmartSearch.Worker.FileMonitorService.Persistence;
using System.Reflection;

namespace SmartSearch.Worker.FileMonitorService.Helpers;

public static class AppStaticValues
{
    private static IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    public static string ConnectionString()
    {
        var conf = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

        if (conf == null) { return string.Empty; }

        return conf;
    }

    public static DbContextOptions<ServiceDbContext> SqlServerDbContextOptions()
    {
        return new DbContextOptionsBuilder<ServiceDbContext>()
            .UseSqlServer(AppStaticValues.ConnectionString())
            .EnableSensitiveDataLogging()
            .Options;
    }

    public static string PublishFolder()
    {
        var conf = configuration.GetSection("FileServer:PublishFolder").Value;

        if (conf == null) { return string.Empty; }

        string[] parts = conf.Split('/');

        return string.Join(Path.DirectorySeparatorChar, parts);
    }

    public static string MonitorFolder()
    {
        var conf = configuration.GetSection("FileServer:MonitorFolder").Value;

        if (conf == null) { return string.Empty; }

        string[] parts = conf.Split('/');

        return string.Join(Path.DirectorySeparatorChar, parts);
    }

    public static string PythonPath()
    {
        var conf = configuration.GetSection("FileServer:PythonPath").Value;

        if (conf == null) { return string.Empty; }

        string[] parts = conf.Split('/');

        return string.Join(Path.DirectorySeparatorChar, parts);
    }

    public static List<string> DocumentExtensions()
    {
        var conf = configuration.GetSection("FileServer:Document:Extensions").Value;

        if (conf == null) { return new List<string>(); }

        var response = conf.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
        
        return response.Select(x => x.ToLower()).ToList();
    }

    public static string DocumentModelScript()
    {
        var conf = configuration.GetSection("FileServer:Document:ModelScript").Value;

        if (conf == null) { return string.Empty; }

        string[] parts = conf.Split('/');

        return string.Join(Path.DirectorySeparatorChar, parts);
    }

    public static List<string> VideoExtensions()
    {
        var conf = configuration.GetSection("FileServer:Video:Extensions").Value;

        if (conf == null) { return new List<string>(); }

        var response = conf.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        return response.Select(x => x.ToLower()).ToList();
    }

    public static string VideoModelScript()
    {
        var conf = configuration.GetSection("FileServer:Video:ModelScript").Value;

        if (conf == null) { return string.Empty; }

        string[] parts = conf.Split('/');

        return string.Join(Path.DirectorySeparatorChar, parts);
    }
}
