using Serilog;
using SmartSearch.Worker.FileMonitorService.Helpers;
using System.Diagnostics;
using System.Text;


namespace SmartSearch.Worker.FileMonitorService.Services;

public static class AIModel
{
    private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    public static string ProcessDocument(string filePath)
    {
        _rwLock.EnterReadLock();

        try
        {
            var pythonPath = AppStaticValues.PythonPath().Replace('\\', '/');
            var scriptPath = AppStaticValues.DocumentModelScript().Replace('\\', '/');
            filePath = filePath.Replace('\\', '/');

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = AppStaticValues.PythonPath(),
                Arguments = string.Format("{0} \"{1}\"", scriptPath, filePath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.GetEncoding("iso-8859-1"),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    var response = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (error == string.Empty && response != string.Empty && response.StartsWith("{"))
                    {
                        return response;
                    }

                    Log.Warning($"(AIModel::ProcessDocument) -- Empty Response -- {Path.GetFileName(filePath)}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(AIModel::ProcessDocument) -- ERROR -- {ex}");
        }
        finally
        {
            _rwLock.ExitReadLock();
        }

        return string.Empty;
    }

    public static string ProcessVideo(string filePath)
    {
        _rwLock.EnterReadLock();

        try
        {
            var pythonPath = AppStaticValues.PythonPath().Replace('\\', '/');
            var scriptPath = AppStaticValues.VideoModelScript().Replace('\\', '/');
            filePath = filePath.Replace('\\', '/');

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = pythonPath,
                Arguments = string.Format("{0} \"{1}\"", scriptPath, filePath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.GetEncoding("iso-8859-1"),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    var response = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (error == string.Empty && response != string.Empty && response.StartsWith("{"))
                    {
                        return response;
                    }

                    Log.Warning($"(AIModel::ProcessVideo) -- Empty Response -- {Path.GetFileName(filePath)}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(AIModel::ProcessVideo) -- ERROR -- {ex}");
        }
        finally
        {
            _rwLock.ExitReadLock();
        }

        return string.Empty;
    }
}