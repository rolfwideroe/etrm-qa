using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ElvizTestUtils;
using NUnit.Framework;
using VizLoginManager;

namespace TestExecute
{
    class CurveServerTemplates
    {
     
        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            IVizAutomatedLogin automatedLogin = (IVizAutomatedLogin)new VizLogin();
            object defaultLogin = automatedLogin.SetDefaultLogin("Vizard", "elviz");

            DirectoryInfo testcompleteDirectoty = new DirectoryInfo("..\\..\\Source\\TestComplete\\CurveServerTemplates");

            foreach (FileInfo file in testcompleteDirectoty.GetFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;

        }
        
        [Test]
        public void TestCurveServer_PriceBookTemplates()
        {
            string elvizVersion = ElvizInstallationUtility.GetElvizVersion().Trim();
            string releaseNumber = elvizVersion.Substring(0, 6);

            const string arguments = "\"..\\..\\Source\\TestComplete\\CurveServerTemplates\\SimpleObjTest.pjs\" -r -e";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 10, "ElvizETRMClient");

            //parse Log
            int numberOfErrors = 0;

            string relativePath = "\\Source\\TestComplete\\CurveServerTemplates\\Log\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string PathToLogFolder = Path.GetFullPath(baseDirectory + relativePath);
            string[] subdirectoryLog = Directory.GetDirectories(PathToLogFolder);

            string logDescription = "\\Description.tcLog";
            string absolutePath = Path.GetFullPath(subdirectoryLog[0] + logDescription);



            XmlDocument doc = new XmlDocument();
            doc.Load(absolutePath); // log XML

            XmlElement root = doc.DocumentElement;
            if (root != null)
            {
                //get error count
                XmlNodeList errorCountNode = root.SelectNodes("/Nodes/Node[@name='root']/Prp[@name='error test count']");
                if (errorCountNode != null)
                {
                    numberOfErrors = Convert.ToInt32(errorCountNode.Item(0).Attributes[2].Value);
                }
            }

            //string pathToLog = @"file://netvs-tfs/mstest/2018.1/CurveServerTemplates_" + elvizVersion + ".mht";
            string pathToLog = @"file://bradydevstorage.file.core.windows.net/bradydevstorage/ETRM/QA/MSTest/" + releaseNumber + "/CurveServerTemplates_" + elvizVersion + ".mht";

            if ((numberOfErrors != 0))
                Assert.Fail(pathToLog);
        }

        private void LaunchTestExecute(string fileName, string arguments, int timeOutMillisecs, string testedProgramName)
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
