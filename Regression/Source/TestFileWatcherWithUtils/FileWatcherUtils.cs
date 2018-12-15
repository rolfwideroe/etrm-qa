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

namespace TestFileWatcherWithUtils
{
    /// <summary>
    /// FileWarcher test's helper
    /// </summary>
    public class FileWatcherUtils
    {
        const string logDirectoryConst = @"{0}\BradyETRM\Integration\CurveImport\Official\Results";
        const string processedDirectoryConst = @"{0}\BradyETRM\Integration\CurveImport\Official\Processed";
        const string quarantineDirectoryConst = @"{0}\BradyETRM\Integration\CurveImport\Official\Quarantined";
        const string watchPathConst = @"{0}\BradyETRM\Integration\CurveImport\Official";
        const string testFilesFolderConst = @"TestFilesCurveImport\";

        public class FileWatcherConfiguration
        {
            public string LogDirectory { get; set; }
            public string ProcessedDirectory { get; set; }
            public string QuarantineDirectory { get; set; }
            public string WatchPath { get; set; }

            public FileWatcherConfiguration(string log, string processed, string quarantine, string watch)
            {
                LogDirectory = log;
                ProcessedDirectory = processed;
                QuarantineDirectory = quarantine;
                WatchPath = watch;
            }
        }

        static List<MessageDetails> MessageDetailsList { get; set; } = new List<MessageDetails>();
        private const bool additionalLogging = false;

        public static FileWatcherConfiguration GetConfiguration()
        {
            string appServerName = ElvizInstallationUtility.GetAppServerName();

            string logDirectory = string.Format(logDirectoryConst, appServerName);
            string processedDirectory = string.Format(processedDirectoryConst, appServerName);
            string quarantineDirectory = string.Format(quarantineDirectoryConst, appServerName);
            string watchPath = string.Format(watchPathConst, appServerName);

            return new FileWatcherConfiguration(logDirectory, processedDirectory, quarantineDirectory, watchPath);
        }

        //create path settings for current filewatcher 
        public static FileWatcherConfiguration currentConfiguration = GetConfiguration();


        static string getTestcaseFolderFilePath(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(fileName, callingClass, $"Current Directory : {Directory.GetCurrentDirectory()}");

            string currentPath = Path.Combine(Directory.GetCurrentDirectory(), testFilesFolderConst);

            return Path.Combine(currentPath, fileName);
        }

        static string getWatchedFolderFilePath(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(fileName, callingClass, $"Watch Path : {currentConfiguration.WatchPath}");

            return Path.Combine(currentConfiguration.WatchPath, fileName);
        }

        static string getQuarantineFolderFilePath(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(fileName, callingClass, $"Watch Path : {currentConfiguration.QuarantineDirectory}");

            return Path.Combine(currentConfiguration.QuarantineDirectory, fileName);
        }

        static string getProcessedFolderFilePath(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(fileName, callingClass, $"Watch Path : {currentConfiguration.ProcessedDirectory}");

            return Path.Combine(currentConfiguration.ProcessedDirectory, fileName);
        }

        static string getLogFolderFilePath(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(fileName, callingClass, $"Watch Path : {currentConfiguration.LogDirectory}");

            return Path.Combine(currentConfiguration.LogDirectory, fileName);
        }

        /// <summary>
        /// Copies file
        /// </summary>
        /// <param name="fileName">The source file location</param>        
        public static void copyFileFromTestCaseFolderToWatchedFolder(string fileName)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

            string fullTestCaseFileName = getTestcaseFolderFilePath(fileName);
            AddMessage(fileName, callingClass, getLogFolderFilePath(fileName));

            string destinationPath = getWatchedFolderFilePath(fileName);
            AddMessage(fileName, callingClass, getLogFolderFilePath(fileName));

            ManageFileStructure(destinationPath);

            File.Copy(fullTestCaseFileName, destinationPath, true);
        }

        /// <summary>
        /// Checks if file has been processed
        /// </summary>
        /// <param name="fileName">FileName - just name without any direcotory information</param>
        /// <returns></returns>
        public static bool isFileProcced(string fileName)
        {
            return File.Exists(getLogFolderFilePath(fileName)) //file processed and result of the processing came
                && !File.Exists(getWatchedFolderFilePath(fileName)) //file was removed from the watched directory
                && (File.Exists(getProcessedFolderFilePath(fileName)) || File.Exists(getQuarantineFolderFilePath(fileName))); //file was placed either in Processed or Quarantine folder
        }

        /// <summary>
        /// Clears possible results from the previous runs
        /// </summary>
        /// <param name="fileName">test case's file name</param>
        public static void clearTestCasePreviousResults(string fileName)
        {

            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

            SystemUtils.DeleteFileIfExist(getProcessedFolderFilePath(fileName));
            AddMessage(fileName, callingClass, getProcessedFolderFilePath(fileName));

            SystemUtils.DeleteFileIfExist(getQuarantineFolderFilePath(fileName));
            AddMessage(fileName, callingClass, getQuarantineFolderFilePath(fileName));

            SystemUtils.DeleteFileIfExist(getLogFolderFilePath(fileName));
            AddMessage(fileName, callingClass, getLogFolderFilePath(fileName));
        }

        /// <summary>
        /// Waits until a testcase file is being processed
        /// </summary>
        /// <param name="fileName"></param>
        public static void waitWhileFileIsBeingProcessed(string fileName)
        {
            while (true)
            {
                if (isFileProcced(fileName))
                    break;

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Checks if test failed
        /// </summary>
        /// <param name="fileName"></param>
        public static bool didTestFail(string fileName)
        {
            return File.Exists(getQuarantineFolderFilePath(fileName));
        }

        /// <summary>
        /// Checks if test succeeded
        /// </summary>
        /// <param name="fileName"></param>
        public static bool didTestSucceed(string fileName)
        {
            return File.Exists(getProcessedFolderFilePath(fileName));
        }

        /// <summary>
        /// Checks if test should fail
        /// </summary>
        /// <param name="testCaseFileName">test case's file name</param>
        /// <returns>True if should fail</returns>
        public static bool shouldTestCaseFail(string testCaseFileName)
        {
            return testCaseFileName.StartsWith("fail", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if test should pass
        /// </summary>
        /// <param name="testCaseFileName">test case's file name</param>
        /// <returns>True if should pass</returns>
        public static bool shouldTestCasePass(string testCaseFileName)
        {
            return testCaseFileName.StartsWith("pass", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get exception's message text from the log file
        /// </summary> 
        public static string getFailedMessageText(string testCaseFileName)
        {
            XDocument xDoc = XDocument.Load(getLogFolderFilePath(testCaseFileName));
            return xDoc.Element("DealResult").Element("Message").Value;
        }

        private static void AddMessage(string fileName, Type callingClass, string filePath)
        {
            if (additionalLogging)
                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Debug, callingClass,
                                   GetCurrentMethodName(), $"Path : {filePath} - file : {fileName}",
                                   "Debugging Reg Test Failures"));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static string GetCurrentMethodName()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);

            return stackFrame.GetMethod().Name;
        }

        private static void ManageFileStructure(string path)
        {
            CheckAndCreateDirectory(currentConfiguration.WatchPath);
            CheckAndCreateDirectory(currentConfiguration.ProcessedDirectory);
            CheckAndCreateDirectory(currentConfiguration.LogDirectory);
            CheckAndCreateDirectory(currentConfiguration.QuarantineDirectory);

            if (File.Exists(path))
                File.Delete(Path.Combine(path));
        }
    }


}

