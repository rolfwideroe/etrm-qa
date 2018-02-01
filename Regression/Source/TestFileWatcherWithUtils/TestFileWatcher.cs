using System;
using System.Collections.Generic;
using ElvizTestUtils;
using NUnit.Framework;
using TestFileWatcherWithUtils;

namespace TestFileWatcherWithUtils
{
    /// <summary>
    /// Tests the directory watcher part of the API - takes files one by one and places it to the watching folder waits 
    /// while it's being processed and then checks the result in appropriate folder (either Processed or Quarantine)
    /// </summary>
    [TestFixture]
    public class TestFileWatcher
    {
       
        /// <param name="fileName">Name of the file to test by the file watcher</param>
        /// 
        private static readonly IEnumerable<string> TestFilesWatcher = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");
        
        [Test, Timeout(1000*100), TestCaseSource("TestFilesWatcher")]       
        public void filePocessingTest(string fileName)
        {
            FileWatcherUtils.clearTestCasePreviousResults(fileName);

            FileWatcherUtils.copyFileFromTestCaseFolderToWatchedFolder(fileName);

            FileWatcherUtils.waitWhileFileIsBeingProcessed(fileName);

            if (FileWatcherUtils.shouldTestCaseFail(fileName) && !FileWatcherUtils.didTestFail(fileName))
                throw new ApplicationException(string.Format("Test {0} should FAIL but PASSED", fileName));
            if (FileWatcherUtils.shouldTestCasePass(fileName) && !FileWatcherUtils.didTestSucceed(fileName))
                throw new ApplicationException(string.Format("Test {0} should PASS but FAILED with message {1}", fileName, FileWatcherUtils.getFailedMessageText(fileName)));
            
        }


        
        
    }
}
