using System;
using System.Diagnostics;
using System.IO;

namespace TestSchedulingManager
{
    public static class SchedulingManagerExecutor
    {
        public static void Run(string pathToSchedulingManager, string arguementsToSchedulingManager)
        {
            try
            {
            String path = Path.Combine(pathToSchedulingManager, "Viz.Integration.Core.SchedulingModule.UI.exe");
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                                                        {
                                                            FileName = path,
                                                            WindowStyle = ProcessWindowStyle.Normal,
                                                            RedirectStandardInput = true,
                                                            RedirectStandardOutput = true,
                                                            UseShellExecute = false,
                                                            Arguments = arguementsToSchedulingManager,
                                                            WorkingDirectory = (pathToSchedulingManager)
                                                        };
                //processStartInfo.FileName = "path to batchfile.bat";
                Process p = Process.Start(processStartInfo);
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
                throw;
            }
        }
    }
}
