using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ElvizTestUtils;
using NUnit.Framework;
using log4net;
using TestElvizUpdateTool.Helpers;

namespace TestElvizUpdateTool
{
    [TestFixture]
    public class EUTJobSystem
    {
        public EUTJobSystem()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            TestcaseName = @"TestFiles\EUTJobs.xml";
            Localfolderpath = @"\TestFiles\TestsInLocalFolder";
            EutJobs = new EutJobs(TestcaseName);
            JobItemsList = EutJobs.TestCaseJobItemsList;
            DestinationPath = @"\\BERSV-FS01\Felles\QA\Regression_EUT\PricesFromLocalFolder\";
            FullTestCaseFilePath = Path.Combine(Directory.GetCurrentDirectory() + Localfolderpath);
            ManageWindowsDirectories = new ManageWindowsDirectories(FullTestCaseFilePath, DestinationPath, true);
            ReportDateHandler = new ReportDateHandler(DateTime.Today.AddDays(-1));
            ReportDate = ReportDateHandler.IgnoreWeekends();
            IsDayLightTime = ReportDateHandler.IsDayLightTime();
        }

        private string DestinationPath { get; }
        private string FullTestCaseFilePath { get; }
        public bool IsDayLightTime { get; }
        private string TestcaseName { get; }
        private string Localfolderpath { get; }
        private EutJobs EutJobs { get; }
        public IEnumerable<JobItem> JobItemsList { get; set; }
        private ManageWindowsDirectories ManageWindowsDirectories { get; }
        private DateTime ReportDate { get; set; }
        private ReportDateHandler ReportDateHandler { get; set; }
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Test, TestCaseSource(nameof(JobItemsList))]
        public void DownloadAndCheckAreaPricesVizPrices(string description)
        {
            var execution = new Execution(description);
            Assert.IsTrue(execution.JobExecuted());
            var evaluation = new Evaluation(TestcaseName, description, ReportDate, IsDayLightTime);
            Assert.IsTrue(evaluation.Result());
        }

        [Test]
        public void TestLocalFolderJob()
        {
            ManageWindowsDirectories.Replenish();
            //Job id for Prices from local folder =35
            var jobid = JobAPI.GetJobsIdByDescription("Prices from local folder", "Historic Data Update Job");

            JobAPI.ExecuteAndAssertJob(jobid, 3600);
        }
    }
}
