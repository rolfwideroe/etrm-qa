using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils;
using NUnit.Framework;

namespace TestElvizUpdateTool
{
 //    [TestFixture]
    public class TestElvizUpdateTool
    {
         private static readonly IEnumerable<string> TestFilesEUT = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

 //       [Test, Timeout(20000 * 1000), TestCaseSource("TestFilesEUT")]
         public void LaunchCommandLineApp(string testFile)
         {
             
                 string arg = File.ReadAllText("TestFiles\\" + testFile);
                    TestEUT(testFile,arg);
         }

        private static readonly IEnumerable<string> LocalFolder = TestCasesFileEnumeratorByFolder.TestCaseFiles("Testfiles\\LocalFolderTests");

    //     [Test, Timeout(1000 * 1000), TestCaseSource("LocalFolder")]
         public void LocalFolderTest(string testFile)
         {
             string arg = File.ReadAllText("TestFiles\\LocalFolderTests\\" + testFile);
             TestEUT(testFile,arg);
         }

         private void TestEUT(string testFile,string argumentText)
         {
             DateTime today = DateTime.Today;
             string today_str = today.ToString("yyyy-MM-dd");
             string logfilename = "log-" + testFile;
             string logpath = Path.Combine(@".\Logs\", today_str,logfilename);
             if(File.Exists(logpath)) File.Delete(logpath);

             try
             {
                 
                 ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine("C:\\elvizclient", "Bin", "ElvizUT.exe"), argumentText)
                 {
                     UseShellExecute = false
                 };

                 Process p = Process.Start(startInfo);
                 p.WaitForExit(); //timeOut
                 //Check to see if the process is still running.
                 if (p.HasExited == false)
                     //Process is still running.
                     //Test to see if the process is hung up.
                     if (p.Responding)
                         //Process was responding; close the main window.
                         p.CloseMainWindow();
                     else
                     {
                         //Process was not responding; force the process to close.
                         p.Kill();
                         Assert.Fail("EUT Process was not responding. Closing...");
                     }
             }
             catch (Exception e)
             {
                 Assert.Fail(e.Message);
             }
             //check log file
          
             Assert.IsTrue(File.Exists(logpath), "Log file doesn't exist.");
             string text = File.ReadAllText(logpath);

             String last = File.ReadLines(logpath).Last();

             StringAssert.EndsWith("Total number of problems: 0", last, text);
             ParseLogFiles(logpath);
         }

         //[Test]
         public void ParseLogFiles(string filename)
         {
            // string filename = @"E:\TFS\Development\QA\Regression\Bin\TestElvizUpdateTool\Logs\2014-10-28\log-params-LocalFolder.txt";
             StreamReader reader = null;
             try
             {
                 reader = File.OpenText(filename);
                 string line;
                 while ((line = reader.ReadLine()) != null)
                 {
                     List<string> myErrorList
                         = new List<string>(new string[]
                     {
                         "A problem occured with", "Not a known/supported", "ERROR: A fatal error",
                         "The server might be down", "login failed" });
                     foreach (string item in myErrorList)
                     {
                         if (line.Contains(item))
                         {
                             Assert.Fail("Log contains errors: " + filename + "\n" + line);
                         }
                     }

                 }
             }
             finally 
             {
                 if(reader!=null) reader.Close();
                 
             }
        
            

         }

    }
}
