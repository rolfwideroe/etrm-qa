using ElvizTestUtils;
using MessageHandler;
using MessageHandler.Pocos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using static ElvizTestUtils.HelperMethods.FileSystemManager;

namespace CurveImport
{
    class CurveImportUtil
    {
        const string logDirectoryConst = @"c:\BradyETRM(Client)\Integration\CurveImport\Official\Results";
        const string processedDirectoryConst = @"c:\BradyETRM(Client)\Integration\CurveImport\Official\Processed";
        const string quarantineDirectoryConst = @"c:\BradyETRM(Client)\Integration\CurveImport\Official\Quarantined";
        const string watchPathConst = @"c:\BradyETRM(Client)\Integration\CurveImport\Official";
        const string testFilesFolderConst = @"TestFilesCurveImport\";
        private const bool additionalLogging = false;

        public class FileWatcherConfigurationCurveImport
        {
            public string CurveLogDirectory { get; set; }
            public string CurveProcessedDirectory { get; set; }
            public string CurveQuarantineDirectory { get; set; }
            public string CurveWatchPath { get; set; }
            public string CurveTestFilesFolder { get; set; }

            public FileWatcherConfigurationCurveImport(string log, string processed, string quarantine, string watch)
            {
                CurveLogDirectory = log;
                CurveProcessedDirectory = processed;
                CurveQuarantineDirectory = quarantine;
                CurveWatchPath = watch;
            }
        }

        static public FileWatcherConfigurationCurveImport GetConfiguration()
        {
            string appServerName = ElvizInstallationUtility.GetAppServerName();

            string logDirectory = string.Format(logDirectoryConst, appServerName);
            string processedDirectory = string.Format(processedDirectoryConst, appServerName);
            string quarantineDirectory = string.Format(quarantineDirectoryConst, appServerName);
            string watchPath = string.Format(watchPathConst, appServerName);

            return new FileWatcherConfigurationCurveImport(logDirectory, processedDirectory, quarantineDirectory, watchPath);
        }
        //create path settings for current filewatcher 
        public static FileWatcherConfigurationCurveImport CurrentCurveImportConfiguration = GetConfiguration();


        public static void PrepareTestFolder(string fileName)
        {
            //clean folders before test
            SystemUtils.DeleteFileIfExist(getProcessedFolderFilePath(fileName));
            SystemUtils.DeleteFileIfExist(getQuarantineFolderFilePath(fileName));
            SystemUtils.DeleteFileIfExist(getLogFolderFilePath(fileName));
            
            //copy files from test case folder to ..\CurveImport\Official
            string fullTestCaseFileName = getTestcaseFolderFilePath(fileName);
            string destinationPath = getWatchedFolderFilePath(fileName);
            CheckAndCreateDirectory(CurrentCurveImportConfiguration.CurveWatchPath);
            File.Copy(fullTestCaseFileName, destinationPath, true);
        }

        // Checks if file has been processed
        public static bool isFileProcced(string fileName)
        {
            return File.Exists(getLogFolderFilePath(fileName)) //file processed and result of the processing came
                && !File.Exists(getWatchedFolderFilePath(fileName)) //file was removed from the watched directory
                && (File.Exists(getProcessedFolderFilePath(fileName)) || File.Exists(getQuarantineFolderFilePath(fileName))); //file was placed either in Processed or Quarantine folder
        }

        public static void WaitWhileFileIsBeingProcessed(string fileName)
        {
            while (true)
            {
                if (isFileProcced(fileName))
                    break;

                Thread.Sleep(600);
            }
        }

        // Checks if test should fail
        // <returns>True if should fail</returns>
        public static bool shouldTestCaseFail(string testCaseFileName)
        {
            return testCaseFileName.StartsWith("Fail", StringComparison.OrdinalIgnoreCase);
        }

        // Checks if test should pass
        // <returns>True if should pass</returns>
        public static bool shouldTestCasePass(string testCaseFileName)
        {
            return testCaseFileName.StartsWith("Pass", StringComparison.OrdinalIgnoreCase);
        }

        // Get exception's message text from the log file
        public static string getFailedMessageText(string testCaseFileName)
        {
            XDocument xDoc = XDocument.Load(getLogFolderFilePath(testCaseFileName));
            return xDoc.Element("ExternalPriceSourceResponseMessage").Element("ErrorMessage").Value;
        }

        // Get exception's message text from the log file
        public static string getTestResult(string testCaseFileName)
        {
            XDocument xDoc = XDocument.Load(getLogFolderFilePath(testCaseFileName));
            return xDoc.Element("ExternalPriceSourceResponseMessage").Element("State").Value;
        }

        // Checks if test failed
        public static bool didTestFail(string fileName)
        {
            return File.Exists(getQuarantineFolderFilePath(fileName));
        }

        // Checks if test succeeded
        public static bool didTestSucceed(string fileName)
        {
            return File.Exists(getProcessedFolderFilePath(fileName));
        }

        static string getTestcaseFolderFilePath(string fileName)
        {
            string currentPath = Path.Combine(Directory.GetCurrentDirectory(), testFilesFolderConst);
            return Path.Combine(currentPath, fileName);
        }

        static string getWatchedFolderFilePath(string fileName)
        {
            return Path.Combine(CurrentCurveImportConfiguration.CurveWatchPath, fileName);
        }

        static string getQuarantineFolderFilePath(string fileName)
        {
            return Path.Combine(CurrentCurveImportConfiguration.CurveQuarantineDirectory, fileName);
        }


        static string getProcessedFolderFilePath(string fileName)
        {
            return Path.Combine(CurrentCurveImportConfiguration.CurveProcessedDirectory, fileName);
        }

        static string getLogFolderFilePath(string fileName)
        {
            string fullFileName = Path.Combine(CurrentCurveImportConfiguration.CurveLogDirectory, fileName);
            string filenameXml = Path.ChangeExtension(fullFileName, "xml");
            return filenameXml;
        }
    }
}
