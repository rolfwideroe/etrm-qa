using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using ElvizTestUtils;
using NUnit.Framework;
using VizLoginManager;

namespace TestExecute
{

    [TestFixture]
    public class DealEntry 
    {

        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";
        //private const string TestExecuteFileName =
        //    "\"C:\\Program Files (x86)\\SmartBear\\TestComplete 9\\Bin\\TestComplete.exe\"";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            IVizAutomatedLogin automatedLogin = (IVizAutomatedLogin)new VizLogin();
            object defaultLogin = automatedLogin.SetDefaultLogin("Vizard", "elviz");

            DirectoryInfo testcompleteDirectoty = new DirectoryInfo("..\\..\\Source\\TestComplete\\DealEntry");

            foreach (FileInfo file in testcompleteDirectoty.GetFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;
           
        }

        //[Test, Order(0)]//update
        //public void TestStartEAM()
        //{  
        //    const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Main|Init -exit";
        //    LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 5, "ElvizTM");
        //    //ParseLogs("RunEAMwithCMDParams");//testName
        // }

        [Test, Order(1)]
        public void TestDealEntry_1CreateDealAndAssertDTO()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|CreateCompareQATransactionDTO|CreateDealAndAssertDTO -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 60, "ElvizTM");
            ParseLogs("CreateDealAndAssertDTO");//testName
        }

        [Test, Order(2)]
        public void TestDealEntry_2Electricity()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Electricity|Electricity -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 360, "ElvizTM");
            ParseLogs("Electricity");//testName
        }

        [Test, Order(3)]
        public void TestDealEntry_3Gas()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Gas|Gas -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 120, "ElvizTM");
            ParseLogs("Gas");//testName
        }

        [Test, Order(4)]
        public void TestDealEntry_Oil()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Oil|Oil -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("Oil");//testName
        }

        [Test, Order(5)]
        public void TestDealEntry_Emission()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Emission|Emission -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("Emission");//testName
        }

        [Test, Order(6)]
        public void TestDealEntry_Elcertificate()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Elcertificate|Elcertificate -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("Elcertificate");//testName
        }

        [Test, Order(7)]
        public void TestDealEntry_Currency()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Currency|Currency -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 60, "ElvizTM");
            ParseLogs("Currency");//testName
        }

        [Test, Order(8)]
        public void TestDealEntry_GreenCertificate()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|GreenCertificate|GreenCertificateForward -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("GreenCertificateForward");//testName
        }

        [Test, Order(9)]
        public void TestDealEntry_Coal()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Coal|Coal -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("Coal");//testName
        }

        [Test, Order(10)]
        public void TestDealEntry_zLogInWithPAM()
        {
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|UsingPAM|LogInWithPAM -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("LogInWithPAM");//testName
        }

        public void LaunchTestExecute(string fileName, string arguments, int timeOutMillisecs, string testedProgramName)
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

        public void ParseLogs(string testName)
        {
            //}
            //[Test]

            //public void ParseLogs2()
            //{
            // string testName = "Electricity";

            int numberOfErrors = 0;
            string elvizVersion = ElvizInstallationUtility.GetElvizVersion().Trim();
            string releaseNumber = elvizVersion.Substring(0, 6);

            string relativePath = "\\Source\\TestComplete\\DealEntry\\DealEntry\\Log\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string PathToLogFolder = Path.GetFullPath(baseDirectory + relativePath);

            string logDescription = "DealEntry.mds.tcLogs";
            string absolutePath = Path.GetFullPath(PathToLogFolder + logDescription);

            string pathToLog = @"file://bradydevstorage.file.core.windows.net/bradydevstorage/ETRM/QA/MSTest/" + releaseNumber + "/DealEntry/" + testName + "_" + elvizVersion + ".mht";

            XmlDocument doc = new XmlDocument();
            doc.Load(absolutePath); // log XML

            XmlElement root = doc.DocumentElement;
            if (root != null)
            {
                XmlNodeList allLogsNode = root.SelectNodes("/Nodes/Node[@name='root']/Node[@name='logs']");
                if (allLogsNode != null)
                {
                    XmlNode xmlNode = allLogsNode.Item(0);
                    if (xmlNode != null)
                    {
                        XmlNodeList logNode = xmlNode.ChildNodes;
                        for (int child = 0; child < logNode.Count; child++)
                        {

                            //check that testname is correspond test execution
                            XmlNodeList nameofrootNode = logNode.Item(child).SelectNodes("Prp[@name='nameofroot']");
                            if (nameofrootNode.Item(0).Attributes["value"].Value.Contains(testName))
                            {
                                XmlNodeList relpathNode = logNode.Item(child).SelectNodes("Prp[@name='relpath']");
                                string testLogFile = PathToLogFolder + relpathNode.Item(0).Attributes["value"].Value;
                                Console.WriteLine(testLogFile);

                                //parse log file for current test
                                XmlDocument log = new XmlDocument();
                                log.Load(testLogFile);
                                XmlElement rootNode = log.DocumentElement;
                                if (rootNode != null)
                                {
                                    //get error count
                                    XmlNodeList errorCountNode =
                                        rootNode.SelectNodes("/Nodes/Node[@name='root']/Prp[@name='error count']");
                                    if (errorCountNode != null)
                                    {
                                        numberOfErrors =
                                            Convert.ToInt32(errorCountNode.Item(0).Attributes["value"].Value);
                                    }

                                }
                            }

                            if ((numberOfErrors != 0)) Assert.Fail(pathToLog);
                        }

                    }
                    else Assert.Fail("Errors when parsing log file for project DealEntry. Test " + testName);

                }
            }
        }


    }
}
