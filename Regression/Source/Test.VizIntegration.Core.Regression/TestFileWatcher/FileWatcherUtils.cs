using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using ElvizTestUtils;
using Test.VizIntegration.Core.Regression.Properties;
using System.Threading;
using System.Diagnostics;

namespace Test.VizIntegration.Core.Regression
{
    /// <summary>
    /// FileWarcher test's helper
    /// </summary>
    public class FileWatcherUtils
    {
        static string getTestcaseFolderFilePath(string processorName, string fileName)
        {
            return Path.Combine(TestCaseConfiguration.FileProcessors[processorName].TestFilesFolder, fileName);
        }

        static string getWatchedFolderFilePath(string processorName, string fileName)
        {
            return Path.Combine(TestCaseConfiguration.FileProcessors[processorName].WatchPath, fileName);
        }

        static string getQuarantineFolderFilePath(string processorName, string fileName)
        {
            return Path.Combine(TestCaseConfiguration.FileProcessors[processorName].QuarantineDirectory, fileName);
        }

        static string getProcessedFolderFilePath(string processorName, string fileName)
        {
            return Path.Combine(TestCaseConfiguration.FileProcessors[processorName].ProcessedDirectory, fileName);
        }

        static string getLogFolderFilePath(string processorName, string fileName)
        {
            return Path.Combine(TestCaseConfiguration.FileProcessors[processorName].LogDirectory, fileName);
        }

        /// <summary>
        /// Copies file
        /// </summary>
        /// <param name="fileName">The source file location</param>        
        public static void copyFileFromTestCaseFolderToWatchedFolder(string processorName, string fileName)
        {
            var fullTestCaseFileName = getTestcaseFolderFilePath(processorName, fileName);
            var destinationPath = getWatchedFolderFilePath(processorName, fileName);

            File.Copy(fullTestCaseFileName, destinationPath);            
        }

        /// <summary>
        /// Checks if file has been processed
        /// </summary>
        /// <param name="fileName">FileName - just name without any direcotory information</param>
        /// <returns></returns>
        public static bool isFileProcced(string processorName, string fileName)
        {
            return File.Exists(getLogFolderFilePath(processorName, fileName)) //file processed and result of the processing came
                && !File.Exists(getWatchedFolderFilePath(processorName, fileName)) //file was removed from the watched directory
                && (File.Exists(getProcessedFolderFilePath(processorName, fileName)) || File.Exists(getQuarantineFolderFilePath(processorName, fileName))); //file was placed either in Processed or Quarantine folder
        }        

        /// <summary>
        /// Clears possible results from the previous runs
        /// </summary>
        /// <param name="fileName">test case's file name</param>
        public static void clearTestCasePreviousResults(string processorName, string fileName)
        {
            SystemUtils.DeleteFileIfExist(getProcessedFolderFilePath(processorName, fileName));
            SystemUtils.DeleteFileIfExist(getQuarantineFolderFilePath(processorName, fileName));
            SystemUtils.DeleteFileIfExist(getLogFolderFilePath(processorName, fileName));            
        }

        /// <summary>
        /// Waits until a testcase file is being processed
        /// </summary>
        /// <param name="fileName"></param>
        public static void waitWhileFileIsBeingProcessed(string processorName, string fileName)
        {
            while (true)
            {
                if (isFileProcced(processorName, fileName))
                    break;

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Checks if test failed
        /// </summary>
        /// <param name="fileName"></param>
        public static bool didTestFail(string processorName, string fileName)
        {
            return File.Exists(getQuarantineFolderFilePath(processorName, fileName));
        }

        /// <summary>
        /// Checks if test succeeded
        /// </summary>
        /// <param name="fileName"></param>
        public static bool didTestSucceed(string processorName, string fileName)
        {
            return File.Exists(getProcessedFolderFilePath(processorName, fileName));
        } 

        /// <summary>
        /// Checks if test should fail
        /// </summary>
        /// <param name="testCaseFileName">test case's file name</param>
        /// <returns>True if should fail</returns>
        public static bool shouldTestCaseFail(string processorName, string testCaseFileName)
        {
            return testCaseFileName.StartsWith("fail", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if test should pass
        /// </summary>
        /// <param name="testCaseFileName">test case's file name</param>
        /// <returns>True if should pass</returns>
        public static bool shouldTestCasePass(string processorName, string testCaseFileName)
        {
            return testCaseFileName.StartsWith("pass", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get exception's message text from the log file
        /// </summary>
        /// <param name="processorName"></param>
        /// <param name="testCaseFileName"></param>
        /// <returns></returns>
        public static string getFailedMessageText(string processorName, string testCaseFileName)
        {
            XDocument xDoc = XDocument.Load(getLogFolderFilePath(processorName, testCaseFileName));
            return xDoc.Element("DealResult").Element("Message").Value;
        }

    }
}
