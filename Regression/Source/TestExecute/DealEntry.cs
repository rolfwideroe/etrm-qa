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
    public class DealEntry : OrderedTestFixture
    {
       // private string elvizVersion;
        private static int MyInt;

        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";
        //private const string TestExecuteFileName =
        //    "\"C:\\Program Files (x86)\\SmartBear\\TestComplete 9\\Bin\\TestComplete.exe\"";

        [TestFixtureSetUp]
        public void SetUp()
        {
            IVizAutomatedLogin automatedLogin = (IVizAutomatedLogin)new VizLogin();
            object defaultLogin = automatedLogin.SetDefaultLogin("Vizard", "elviz");

            DirectoryInfo testcompleteDirectoty = new DirectoryInfo("..\\..\\Source\\TestComplete\\DealEntry");

            foreach (FileInfo file in testcompleteDirectoty.GetFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;
            MyInt = 0;

        }
       
        [TestCaseSource(sourceName: "TestSource")]
        public void DealEntrySolution(TestStructure test)
        {
            test.Test();
        }

        ////[Test]
        //[OrderedTest(0)]
        //public void TestStartEAM()
        //{
        //    MyInt++;
        //    const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Main|Init -exit";
        //    LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 5, "ElvizTM");
        //    //ParseLogs("RunEAMwithCMDParams");//testName
        // }

        [OrderedTest(0)]
        public void TestDealEntry_CreateDealAndAssertDTO()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|CreateCompareQATransactionDTO|CreateDealAndAssertDTO -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 60, "ElvizTM");
            ParseLogs("CreateDealAndAssertDTO");//testName
        }

        [OrderedTest(1)]
        public void TestDealEntry_Electricity()
        {
            Console.WriteLine(DateTime.Now);
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Electricity|Electricity -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 360, "ElvizTM");
            ParseLogs("Electricity");//testName
        }

        [OrderedTest(2)]
        public void TestDealEntry_Gas()
        {
            Console.WriteLine(DateTime.Now);
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Gas|Gas -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 120, "ElvizTM");
            ParseLogs("Gas");//testName
        }

        [OrderedTest(3)]
        public void TestDealEntry_Oil()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Oil|Oil -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("Oil");//testName
        }

        [OrderedTest(4)]
        public void TestDealEntry_Emission()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Emission|Emission -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("Emission");//testName
        }

        [OrderedTest(5)]
        public void TestDealEntry_Elcertificate()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Elcertificate|Elcertificate -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("Elcertificate");//testName
        }

        [OrderedTest(6)]
        public void TestDealEntry_Currency()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Currency|Currency -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 60, "ElvizTM");
            ParseLogs("Currency");//testName
        }

        [OrderedTest(7)]
        public void TestDealEntry_GreenCertificate()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|GreenCertificate|GreenCertificateForward -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("GreenCertificateForward");//testName
        }

        [OrderedTest(8)]
        public void TestDealEntry_Coal()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|Coal|Coal -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizTM");
            ParseLogs("Coal");//testName
        }

        [OrderedTest(9)]
        public void TestDealEntry_LogInWithPAM()
        {
            MyInt++;
            const string arguments = "\"..\\..\\Source\\TestComplete\\DealEntry\\DealEntry.pjs\" -r -p:DealEntry /test:Script|UsingPAM|LogInWithPAM -exit";
            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 15, "ElvizTM");
            ParseLogs("LogInWithPAM");//testName
        }

        public void LaunchTestExecute(string fileName, string arguments, int timeOutMillisecs,string testedProgramName)
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

                    Assert.Fail("Test Execute has timed out, exceeded "+new TimeSpan(0,0,0,0,timeOutMillisecs));
    
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
            
            //[Test]
            //public void ParseLogs2()
            //{
            //    string testName = "Currency";
            //parse Log
            int numberOfErrors = 0;
            string elvizVersion = ElvizInstallationUtility.GetElvizVersion();

            string relativePath = "\\Source\\TestComplete\\DealEntry\\DealEntry\\Log\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string PathToLogFolder = Path.GetFullPath(baseDirectory + relativePath);

            string logDescription = "DealEntry.mds.tcLogs";
            string absolutePath = Path.GetFullPath(PathToLogFolder + logDescription);
            Console.WriteLine("elvizVersion = " + elvizVersion);
            string pathToLog = @"file://netvs-tfs/mstest/2017.2/DealEntry/"+testName + "_" + elvizVersion + ".mht";

            Console.WriteLine(absolutePath);

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
