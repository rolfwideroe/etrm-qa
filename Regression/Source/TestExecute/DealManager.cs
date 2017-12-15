using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using VizLoginManager;

namespace TestExecute
{
    class Deal_Manager : OrderedTestFixture
    {
        private static int MyInt;

        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";
        //private const string TestExecuteFileName =
        //    "\"C:\\Program Files (x86)\\SmartBear\\TestComplete 9\\Bin\\TestComplete.exe\"";

        [TestFixtureSetUp]
        public void SetUp()
        {
            IVizAutomatedLogin automatedLogin = (IVizAutomatedLogin)new VizLogin();
            object defaultLogin = automatedLogin.SetDefaultLogin("Vizard", "elviz");

            DirectoryInfo testcompleteDirectoty = new DirectoryInfo("..\\..\\Source\\TestComplete\\DealManager");

            foreach (FileInfo file in testcompleteDirectoty.GetFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;

            MyInt = 0;

        }

        [TestCaseSource(sourceName: "TestSource")]
        public void DealManagerSolution(TestStructure test)
        {
            test.Test();
        }

        //[Test]
        [OrderedTest(0)]
        public void TestDealManager_StartEAM()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|DealManager|Init -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 5, "ElvizTM");
            ParseLogs("Init");//testName
        }

     
        //[Test]
        [OrderedTest(1)]
        public void TestDealManager_ChangeStatus()
        {
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|Status|ChangeStatus -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("ChangeStatus");//testName
        }

        //[Test]
        [OrderedTest(2)]
        public void TestDealManager_CopyTransaction()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|CopyDeal|CopyTransaction -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("CopyTransaction");//testName
        }

        //[Test]
        [OrderedTest(3)]
        public void TestDealManager_UpdateTransaction()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|DealManager|UpdateTransactions -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 30, "ElvizTM");
            ParseLogs("UpdateTransactions");//testName
        }

        ////[Test]
        //[OrderedTest(4)]
        //public void TestDealManager_CopyAndUpdateTransaction()
        //{
        //    MyInt++;
        //    string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|CopyDeal|CopyAndUpdateTransaction -exit";

        //    LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 30, "ElvizTM");
        //    ParseLogs("CopyAndUpdateTransaction");//testName
        //}

        //[Test]
        [OrderedTest(5)]
        public void TestDealManager_ImportFSDTimeSeries()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|ImportFSDTimeseries|ImportFSDTimeSeries -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 10, "ElvizTM");
            ParseLogs("ImportFSDTimeSeries");//testName
        }

        //[Test]
        [OrderedTest(6)]
        public void TestDealManager_EmirTimestamps()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|EMIR|EMIRTimestamps -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("EMIRTimestamps");//testName
        }

        //[Test]
        [OrderedTest(7)]
        public void TestDealManager_MultipleUpdate()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|BulkUpdate|MultipleUpdate -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 10, "ElvizTM");
            ParseLogs("MultipleUpdate");//testName
        }

        //[Test]
        [OrderedTest(8)]
        public void TestDealManager_TradeTo()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|TradeTo|TradeTo -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 10, "ElvizTM");
            ParseLogs("TradeTo");//testName
        }

        //[Test]
        [OrderedTest(9)]
        public void TestDealManager_Distribution()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|Distribution|Distribution -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("Distribution");//testName
        }

        //[Test]
        [OrderedTest(10)]
        public void TestDealManager_TimezoneTest()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\DealManager\\DealManager.pjs\" -r -p:DealManagerFunctions /test:Script|TimeZones|RunTimezoneTest -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 10, "ElvizTM");
            ParseLogs("RunTimezoneTest");//testName
        }


        public void ParseLogs(string testName)
        {

            int numberOfErrors = 0;
            string elvizVersion = ElvizInstallationUtility.GetElvizVersion();

            string relativePath = "\\Source\\TestComplete\\DealManager\\DealManager\\Log\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string PathToLogFolder = Path.GetFullPath(baseDirectory + relativePath);

            string logDescription = "DealManagerFunctions.mds.tcLogs";
            string absolutePath = Path.GetFullPath(PathToLogFolder + logDescription);
            string pathToLog = @"file://netvs-tfs/mstest/2018.1/DealManager/" + testName + "_" + elvizVersion + ".mht";

            //Console.WriteLine(absolutePath);

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
                                //Console.WriteLine(testLogFile);

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

                            if (numberOfErrors != 0) Assert.Fail(pathToLog);
                        }

                    }
                    else Assert.Fail("Errors when parsing log file for project DealManager. Test " + testName);

                }
            }
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
