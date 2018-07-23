using System.IO;
using System.Text;
using System.Threading;

namespace TestElvizUpdateTool.Helpers
{
    public class ManageWindowsDirectories
    {
        public ManageWindowsDirectories(string sourceDirectoryName, string destinationDirectoryName, bool copySubDirectories)
        {
            SourceDirectoryName = sourceDirectoryName;
            DestinationDirectoryName = destinationDirectoryName;
            CopySubDirectories = copySubDirectories;
        }

        string SourceDirectoryName { get; }
        string DestinationDirectoryName { get; }
        bool CopySubDirectories { get; }
        public static int ProcessTimeOut { get; } = 10000;

        public void Replenish()
        {
            DirectoryDelete();
            CreateDirectoryAsNew();
            if(CopySubDirectories) DirectoryCopy();
        }

        private void DirectoryDelete()
        {
            if (!Directory.Exists(DestinationDirectoryName)) return;
            Directory.Delete(DestinationDirectoryName, true);
            Thread.Sleep(100);
        }

        private void CreateDirectoryAsNew()
        {
            if (Directory.Exists(DestinationDirectoryName)) return;
            Directory.CreateDirectory(DestinationDirectoryName);
            Thread.Sleep(100);
        }

        private void DirectoryCopy()
        {
            var process = new System.Diagnostics.Process();
            var startInfo =
                new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Verb = "runas",
                    WorkingDirectory = DestinationDirectoryName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };
            process.StartInfo = startInfo;
            process.Start();

            process.StandardInput.WriteLine(BuildCommand());

            process.WaitForExit(ProcessTimeOut);
        }

        private string BuildCommand()
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.Append("xcopy /h/i/c/k/e/r/y ");
            commandBuilder.Append(SourceDirectoryName);
            commandBuilder.Append(@"\*.* ");
            commandBuilder.Append(DestinationDirectoryName);

            return commandBuilder.ToString();

        }
    }
}
