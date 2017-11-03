using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace TestMembershipServiceLogin
{
    [Binding]
    public class TestExecuteWrapper
    {
        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";
        //private const string TestExecuteFileName =
        //    "\"C:\\Program Files (x86)\\SmartBear\\TestComplete 9\\Bin\\TestComplete.exe\"";
      //  [Test]
       public static void MembershipLoginWindowAppeared()
        {

           var testDirContext = TestContext.CurrentContext.TestDirectory;
           string regressionFolder = Path.GetFullPath(Path.Combine(testDirContext, @"../.."));
            
           //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\PriceImportInterestRates" + elvizVersion + ".mht";
            //            string arguments = "..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:MembershipServiceLogin -exit";
            
           string arguments = "\"" + regressionFolder+ "\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:MembershipServiceLogin -exit";
  
           LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 5, "ElvizETRMClient");

            //ParseLogs("PriceImportInterestRates");
        }

        private static void LaunchTestExecute(string fileName, string arguments, int timeOutMillisecs, string testedProgramName)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false
            };

            Process p = Process.Start(startInfo);
            bool hasFinishd = p.WaitForExit(timeOutMillisecs); //timeOut
            if (!hasFinishd)
            {
                p.Kill();

                Process[] testedProgramPrs = Process.GetProcessesByName(testedProgramName);

                foreach (Process process in testedProgramPrs)
                {
                    process.Kill();
                }
                Assert.Fail("Test Execute has timed out, exceeded " + new TimeSpan(0, 0, 0, 0, timeOutMillisecs));
            }

            if (p.HasExited == false)
            {
                p.Kill();
                Console.WriteLine("Process was not responding. Closing...");
                Assert.Fail("Had to Kill process");
            }


        }

    }
}
