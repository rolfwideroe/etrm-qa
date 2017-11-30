using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using ElvizTestUtils;

namespace TestFileWatcherWithUtils
{
   /// <summary>
    /// FileWarcher test's helper
    /// </summary>
    public class FileWatcherUtils
    {
            const string logDirectoryConst = @"\\{0}\BradyETRM\Integration\DealImport\Results";
            const string processedDirectoryConst = @"\\{0}\BradyETRM\Integration\DealImport\Processed";
            const string quarantineDirectoryConst = @"\\{0}\BradyETRM\Integration\DealImport\Quarantined";
            const string watchPathConst = @"\\{0}\BradyETRM\Integration\DealImport";
            const string testFilesFolderConst = @"TestFiles\";
           
        public class FileWatcherConfiguration
        {
            public string LogDirectory { get; set; }
            public string ProcessedDirectory { get; set; }
            public string QuarantineDirectory { get; set; }
            public string WatchPath { get; set; }
            public string TestFilesFolder { get; set; }

            public FileWatcherConfiguration(string log, string processed, string quarantine, string watch)
            {
                LogDirectory = log;
                ProcessedDirectory = processed;
                QuarantineDirectory = quarantine;
                WatchPath = watch;
            }
        }

        static public FileWatcherConfiguration GetConfiguration()
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
           string currentPath = Path.Combine(Directory.GetCurrentDirectory(), testFilesFolderConst);
           return Path.Combine(currentPath, fileName);
       }

       static string getWatchedFolderFilePath(string fileName)
       {
           return Path.Combine(currentConfiguration.WatchPath, fileName);
       }

       static string getQuarantineFolderFilePath(string fileName)
       {
           return Path.Combine(currentConfiguration.QuarantineDirectory, fileName);
       }

       static string getProcessedFolderFilePath(string fileName)
       {
           return Path.Combine(currentConfiguration.ProcessedDirectory, fileName);
       }

       static string getLogFolderFilePath(string fileName)
       {
           return Path.Combine(currentConfiguration.LogDirectory, fileName);
       }

       /// <summary>
       /// Copies file
       /// </summary>
       /// <param name="fileName">The source file location</param>        
       public static void copyFileFromTestCaseFolderToWatchedFolder(string fileName)
       {
           string fullTestCaseFileName = getTestcaseFolderFilePath(fileName);
           string destinationPath = getWatchedFolderFilePath(fileName);

           File.Copy(fullTestCaseFileName, destinationPath);
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
               && (File.Exists(getProcessedFolderFilePath(fileName)) || File.Exists(getQuarantineFolderFilePath( fileName))); //file was placed either in Processed or Quarantine folder
       }

       /// <summary>
       /// Clears possible results from the previous runs
       /// </summary>
       /// <param name="fileName">test case's file name</param>
       public static void clearTestCasePreviousResults(string fileName)
       {
           SystemUtils.DeleteFileIfExist(getProcessedFolderFilePath(fileName));
           SystemUtils.DeleteFileIfExist(getQuarantineFolderFilePath(fileName));
           SystemUtils.DeleteFileIfExist(getLogFolderFilePath(fileName));
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

    }

   
}

   