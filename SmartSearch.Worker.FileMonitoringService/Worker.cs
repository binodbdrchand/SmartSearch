using SmartSearch.Worker.FileMonitorService.Helpers;
using SmartSearch.Worker.FileMonitorService.Services;

namespace SmartSearch.Worker.FileMonitorService
{
    public class Worker : BackgroundService
    {
        public Worker()
        {
            CreateApplicationDirectories.CreateUploadDirectory();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => 
            {
                _ = new FileWatcher(AppStaticValues.MonitorFolder());
            }, 
            stoppingToken);
        }
    }
}