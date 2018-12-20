using ElvizTestUtils;
using MessageHandler.Pocos;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [Test, TestCaseSource("TestFilesGenConExport")]
        public void TestGenCon(string testFile)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs\\");
            CheckAndCreateDirectory(path);
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            string testFilePathCFIN = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\CF\\" + Path.GetFileNameWithoutExtension(testFile) + "_CF_IN.xml");
            string testFilePathCFOUT = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\CF\\" + Path.GetFileNameWithoutExtension(testFile) + "_CF_OUT.xml");
            string exportFilePath = Path.Combine(@"c:\BradyETRM(Client)\Integration", @"ElvizEntityExportParameters\");
            CheckAndCreateDirectory(exportFilePath);
            string exportLocalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExportFiles\\");
            CheckAndCreateDirectory(exportLocalFilePath);

            string localPath;
            string localTestfile;
            string localTestFilePath;

            File.SetAttributes(path, FileAttributes.Normal);
            File.SetAttributes(exportFilePath, FileAttributes.Normal);
            File.SetAttributes(exportLocalFilePath, FileAttributes.Normal);
            string[] exportFiles = Directory.GetFiles(exportFilePath, "*_" + Path.GetFileNameWithoutExtension(testFile) + "_*.xml");
            foreach (string f in exportFiles)
            {
                //deletes the files for the filter
                File.Delete(f);
            }

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
    }
}
