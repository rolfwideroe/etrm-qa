using ElvizTestUtils;
using MessageHandler;
using MessageHandler.Pocos;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static ElvizTestUtils.HelperMethods.FileSystemManager;

namespace TestGenConExport
{
    [TestFixture]
    public class TestGenConExport
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        //test gencon exports
        // in \TestFiles need a file per GenCon filter name
        // the tests will run 

        private static readonly IEnumerable<string> TestFilesGenConExport = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");
        static List<MessageDetails> MessageDetailsList { get; set; } = new List<MessageDetails>();

        private const bool additionalLogging = false;

        [Test, TestCaseSource("TestFilesGenConExport")]
        public void TestGenCon(string testFile)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs\\");

            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage(testFile, callingClass, $"Logs Path : {path}");

            CheckAndCreateDirectory(path);

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            AddMessage(testFile, callingClass, $"testFilePath : {testFilePath}");

            string testFilePathCFIN = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\CF\\" + Path.GetFileNameWithoutExtension(testFile) + "_CF_IN.xml");
            AddMessage(testFile, callingClass, $"testFilePathCFIN : {testFilePathCFIN}");

            string testFilePathCFOUT = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\CF\\" + Path.GetFileNameWithoutExtension(testFile) + "_CF_OUT.xml");
            AddMessage(testFile, callingClass, $"testFilePathCFOUT : {testFilePathCFOUT}");

            string exportFilePath = Path.Combine("\\\\" + ElvizTestUtils.ElvizInstallationUtility.GetAppServerName(),
                            "BradyETRM\\Integration", "ElvizEntityExportParameters\\");
            AddMessage(testFile, callingClass, $"exportFilePath : {exportFilePath}");

            CheckAndCreateDirectory(exportFilePath);

            string exportLocalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExportFiles\\");
            AddMessage(testFile, callingClass, $"exportLocalFilePath : {exportLocalFilePath}");

            CheckAndCreateDirectory(exportLocalFilePath);

            string localPath;
            string localTestfile;
            string localTestFilePath;

            File.SetAttributes(path, FileAttributes.Normal);
            File.SetAttributes(exportFilePath, FileAttributes.Normal);
            File.SetAttributes(exportLocalFilePath, FileAttributes.Normal);

            AddMessage(testFile, callingClass, $"exportFilePath : {exportFilePath}");

            string[] exportFiles = Directory.GetFiles(exportFilePath, "*_" + Path.GetFileNameWithoutExtension(testFile) + "_*.xml");
            foreach (string f in exportFiles)
            {
                //deletes the files for the filter
                File.Delete(f);
            }

            AddMessage(testFile, callingClass, $"exportLocalFilePath : {exportLocalFilePath}");

            string[] exportLocalFiles = Directory.GetFiles(exportLocalFilePath, "*_" + Path.GetFileNameWithoutExtension(testFile) + "_*.xml");
            foreach (string f in exportLocalFiles)
            {
                //deletes the files for the filter
                File.Delete(f);
            }

            if (!GenConExportUtils.ExportGenCon(exportFilePath, Path.GetFileNameWithoutExtension(testFile)))
            {
                Assert.Fail("The GenConExport failed." + "File: " + exportFilePath);
            }

            //get the exported file
            exportFiles = Directory.GetFiles(exportFilePath, "*_" + Path.GetFileNameWithoutExtension(testFile) + "_*.xml");
            //if CF
            if (Path.GetFileNameWithoutExtension(testFile).EndsWith("-CF") && (exportFiles.Count() == 3))
            {
                foreach (string exportFile in exportFiles)
                {
                    localPath = string.Empty;
                    localTestfile = string.Empty;
                    localTestFilePath = string.Empty;
                    if (exportFile.Contains("-CF_Pass"))
                    {
                        localPath = exportLocalFilePath + Path.GetFileName(exportFile);
                        localTestfile = testFile;
                        localTestFilePath = testFilePath;
                    }
                    else if (exportFile.Contains("-CF_CF_IN"))
                    {
                        localPath = exportLocalFilePath + Path.GetFileName(exportFile);
                        localTestfile = Path.GetFileNameWithoutExtension(testFile) + "_CF_IN.xml";
                        localTestFilePath = testFilePathCFIN;
                    }
                    else if (exportFile.Contains("-CF_CF_OUT"))
                    {
                        localPath = exportLocalFilePath + Path.GetFileName(exportFile);
                        localTestfile = Path.GetFileNameWithoutExtension(testFile) + "_CF_OUT.xml";
                        localTestFilePath = testFilePathCFOUT;
                    }
                    File.Move(exportFile, localPath);
                    if (!GenConExportUtils.XMLDiffCompare(path, localPath, localTestFilePath, localTestfile))
                    {
                        Assert.Fail("The GenConExport does not match the expected. See Logs folder for details");
                    }
                }
            }
            else
            {
                if (exportFiles.Count() != 1)
                {
                    if (testFile.StartsWith("Fail-"))
                    {
                        //OK test as ecpect to fail
                        return;
                    }
                    Assert.Fail("The GenConExport failed. No file:" + exportFilePath);
                }
                else
                {
                    exportFilePath = exportFiles[0];
                }
                localPath = exportLocalFilePath + Path.GetFileName(exportFilePath);

                AddMessage(testFile, callingClass, $"localPath : {localPath}");

                File.Move(exportFilePath, localPath);
                if (!GenConExportUtils.XMLDiffCompare(path, localPath, testFilePath, testFile))
                {
                    if (testFile.StartsWith("Fail-"))
                    {
                        //OK test as ecpect to fail
                        return;
                    }
                    Assert.Fail("The GenConExport does not match the expected. See Logs folder for details");
                }
            }
        }

        private static void AddMessage(string fileName, Type callingClass, string filePath)
        {
            if(additionalLogging)
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
    }
}
