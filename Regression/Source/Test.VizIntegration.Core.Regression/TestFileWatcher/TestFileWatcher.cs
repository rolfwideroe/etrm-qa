using System;
using NUnit.Framework;
using ElvizTestUtils;

namespace Test.VizIntegration.Core.Regression
{
    /// <summary>
    /// Tests the directory watcher part of the API - takes files one by one and places it to the watching folder waits 
    /// while it's being processed and then checks the result in appropriate folder (either Processed or Quarantine)
    /// </summary>
    [TestFixture]
    public class TestFileWatcher
    {
              

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="fileName">Name of the file to test by the file watcher</param>
        [Test, Timeout(1000*100), TestCaseSource(typeof(TestCasesFileEnumerator), "TestCaseFiles")]       
        public void filePocessingTest(string processorName, string fileName)
        {
            FileWatcherUtils.clearTestCasePreviousResults(processorName, fileName);

            FileWatcherUtils.copyFileFromTestCaseFolderToWatchedFolder(processorName, fileName);            

            FileWatcherUtils.waitWhileFileIsBeingProcessed(processorName, fileName);

            if (FileWatcherUtils.shouldTestCaseFail(processorName, fileName) && !FileWatcherUtils.didTestFail(processorName, fileName))
                throw new ApplicationException(string.Format("Test {0} should FAIL but PASSED", fileName));
            if (FileWatcherUtils.shouldTestCasePass(processorName, fileName) && !FileWatcherUtils.didTestSucceed(processorName, fileName))
                throw new ApplicationException(string.Format("Test {0} should PASS but FAILED with message {1}", fileName, FileWatcherUtils.getFailedMessageText(processorName, fileName)));
            
        }


        
        
    }
}
