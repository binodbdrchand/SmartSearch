namespace SmartSearch.Worker.FileMonitorService.Helpers;

public static class CreateApplicationDirectories
{
    public static void CreateUploadDirectory()
    {
        try
        {
            Directory.CreateDirectory(AppStaticValues.PublishFolder() + Path.DirectorySeparatorChar + "AIModel");
            Directory.CreateDirectory(AppStaticValues.PublishFolder() + Path.DirectorySeparatorChar + "FileMonitorService");
            Directory.CreateDirectory(AppStaticValues.PublishFolder() + Path.DirectorySeparatorChar + "WebMvc");

            Directory.CreateDirectory(AppStaticValues.MonitorFolder() + Path.DirectorySeparatorChar + "Documents");
            Directory.CreateDirectory(AppStaticValues.MonitorFolder() + Path.DirectorySeparatorChar + "Videos");
        }
        catch
        {
        }
    }
}
