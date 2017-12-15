using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using NUnit.Framework;
using VizLoginManager;

namespace TestExecute
{
    class CurveServer : OrderedTestFixture
    {
        private string elvizVersion;
        private static int MyInt;

        const string TestExecuteFileName = "\"C:\\Program Files (x86)\\SmartBear\\TestExecute 9\\Bin\\TestExecute.exe\"";
        //private const string TestExecuteFileName =
        //    "\"C:\\Program Files (x86)\\SmartBear\\TestComplete 9\\Bin\\TestComplete.exe\"";

        [TestFixtureSetUp]
        public void SetUp()
        {
          //  IVizAutomatedLogin automatedLogin = (IVizAutomatedLogin)new VizLogin();
           // object defaultLogin = automatedLogin.SetDefaultLogin("Vizard", "elviz");

            DirectoryInfo testcompleteDirectoty = new DirectoryInfo("..\\..\\Source\\TestComplete\\CurveServer");

            foreach (FileInfo file in testcompleteDirectoty.GetFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;

            elvizVersion = ElvizInstallationUtility.GetElvizVersion();

            string guiRegressionSysTradeDate = "2013-06-10";
            QaDao.SetSysTradeDate(guiRegressionSysTradeDate);

            MyInt = 0;

        }
      
        [TestCaseSource(sourceName: "TestSource")]
        public void CurveServerSolution(TestStructure test)
        {
            test.Test();
        }

        //[Test]
        [OrderedTest(0)]
        public void TestCurveServer_Adapter()
        {
            //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\Adapter_" + elvizVersion + ".mht";
            //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\Adapter_" + elvizVersion + "_.mht";

            //if (File.Exists(logName)) File.Move(logName, logName);


            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:Adapter -exit";
            Console.WriteLine(arguments);

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");
            ParseLogs("Adapter");
        }

        //[Test]
        [OrderedTest(1)]
        public void TestCurveServer_PriceImportInterestRates()
        {
            MyInt++;
            //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\PriceImportInterestRates" + elvizVersion + ".mht";
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:PriceImportInterestRates -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");

            ParseLogs("PriceImportInterestRates");
        }


        //[Test]
        [OrderedTest(2)]
        public void TestCurveServer_FXandInterestRates()
        {
            MyInt++;
            //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\FXandInterestRates" + elvizVersion + ".mht";
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:FXandInterestRates -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");

            ParseLogs("FXandInterestRates");
        }


        //[Test]
        [OrderedTest(3)]
        public void TestCurveServer_Jobs()
        {
            MyInt++;
            //string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\JobScheduling" + elvizVersion + ".mht";
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:Jobs -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");

            ParseLogs("Jobs");
        }

        [OrderedTest(4)]
        public void TestCurveServer_EditRepublishPricebooks()
        {
            MyInt++;
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:EditRepublishPriceBooks -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 30, "ElvizETRMClient");

            ParseLogs("EditRepublishPriceBooks");
        }

        //[Test]
        [OrderedTest(5)]
        public void TestCurveServer_Templates()
        {
            MyInt++;
            // string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\Templates" + elvizVersion + ".mht";
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:Templates -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");

            ParseLogs("Templates");
        }


        //[Test]
        [OrderedTest(6)]
        public void TestCurveServer_Configuration()
        {
            MyInt++;
            // string logName = "\\\\Netvs-tfs\\MSTest\\2015.3\\CurveServer\\Configuration" + elvizVersion + ".mht";
            string arguments = "\"..\\..\\Source\\TestComplete\\CurveServer\\CurveServer.pjs\" -r -p:Configuration -exit";

            LaunchTestExecute(TestExecuteFileName, arguments, 1000 * 60 * 20, "ElvizETRMClient");

            ParseLogs("Configuration");
        }

//       [Test]
        public void ParseLogs(string projectName)
        {
           // string projectName = "PriceImportInterestRates";
           //parse Log
            int numberOfErrors = 0;

            string relativePath = "\\Source\\TestComplete\\CurveServer\\" + projectName + "\\Log\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string PathToLogFolder = Path.GetFullPath(baseDirectory + relativePath);
         
            string logDescription = projectName + ".mds.tcLogs";
            string absolutePath = Path.GetFullPath(PathToLogFolder + logDescription);

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
                       XmlNode logNode = xmlNode.LastChild;
                       XmlNodeList relpathNode = logNode.SelectNodes("Prp[@name='relpath']");
                       //Console.WriteLine(relpathNode.Item(0).Attributes["value"].Value);
                       string testLogFile = PathToLogFolder + relpathNode.Item(0).Attributes["value"].Value;
                       //Console.WriteLine(testLogFile);
                       //parse log file for current test

                       XmlDocument log = new XmlDocument();
                       log.Load(testLogFile); 
                       XmlElement rootNode = log.DocumentElement;
                       if (rootNode != null)
                       {
                           //get error count
                           XmlNodeList errorCountNode = rootNode.SelectNodes("/Nodes/Node[@name='root']/Prp[@name='error count']");
                           if (errorCountNode != null)
                           {
                               numberOfErrors = Convert.ToInt32(errorCountNode.Item(0).Attributes["value"].Value);
                           }
                       }
                       string pathToLog = @"file://netvs-tfs/mstest/2018.1/CurveServer/"+ projectName+ "_" + elvizVersion + ".mht";

                       if ((numberOfErrors != 0)) Assert.Fail(pathToLog);
                   }
                   else Assert.Fail("Errors when parsing log file for project " + projectName); 

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
