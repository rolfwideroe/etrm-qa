using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using ElvizTestUtils.InternalJobService;

namespace TestElvizUpdateTool
{
    [TestFixture]
    class EUTJobSystem
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        private static string testcasename = @"TestFiles\EUTJobs.xml";
        private static string localfolderpath = @"\TestFiles\TestsInLocalFolder";

      
        private static List<string> EUTJobDescriptions = GetList();

        public static List<string> GetList()
        {
            string testcasefile = Path.Combine(Directory.GetCurrentDirectory(), testcasename);

            List<string> list=new List<string>();
            JobsTestCase Jobs = TestXmlTool.Deserialize<JobsTestCase>(testcasefile);
            foreach (JobItem item in Jobs.JobItems)
            {
                Console.WriteLine(item.Description + "=" + item.JobId);
                list.Add(item.Description);
            }

            return list;
        }
    
        [Test, TestCaseSource("EUTJobDescriptions")]
        public void DownloadAndCheckAreaPricesVizPrices(string description)
        {
            int jobid = JobAPI.GetJobsIdByDescription(description, "Historic Data Update Job");
            JobExecutionStatus status = JobAPI.ExecuteJob(jobid, null, 800);
            
            ////if debug query ***
            //JobExecutionStatus status = new JobExecutionStatus { Status = "Success" };

            if (status.Status != "Success")
            {
                JobExecutionStatus statusSecondRun = null;
                statusSecondRun = JobAPI.ExecuteJob(jobid, null, 800);

                if (statusSecondRun == null || statusSecondRun.Status != "Success")
                    JobAPI.ExecuteAndAssertJob(jobid, null, 800);
            }

            List<string> errorList = new List<string>();
            
            DateTime reportDate = DateTime.Today.AddDays(-1);

            if (reportDate.DayOfWeek == DayOfWeek.Sunday) reportDate = reportDate.AddDays(-2);
            if (reportDate.DayOfWeek == DayOfWeek.Saturday) reportDate = reportDate.AddDays(-1);
   
            string testcasefile = Path.Combine(Directory.GetCurrentDirectory(), testcasename);

            JobsTestCase jobs = TestXmlTool.Deserialize<JobsTestCase>(testcasefile);

            JobItem item = jobs.JobItems.Single(x => x.Description == description);
            if (item != null)
            {
                string executionVenue = item.ExecutionVenue;
                InstrumentPrice[] instrumentPrices = item.InstrumentPrices;
                SpotPrice[] spotPrices = item.SpotPrices;

                if (instrumentPrices != null && executionVenue != null)
                {
                    foreach (InstrumentPrice areaprice in instrumentPrices)
                    {
                        string currentInstrumentArea = areaprice.Area;
                        DataTable results = QaDao.GetInstrumentPricesByArea(executionVenue, currentInstrumentArea, reportDate.ToString("yyyy-MM-dd"), areaprice.CfdArea);
                        if (results.Rows.Count < 1)
                            errorList.Add("Query did not return any instrument price record for report date= " + 
                                reportDate + ", price area = " + currentInstrumentArea + ", execution venue =" + executionVenue);
                    }
                }

                if (spotPrices != null)
                {
                    foreach (SpotPrice areaprice in spotPrices)
                    {
                        string currentInstrumentArea = areaprice.Area;
                        string currentResolution = areaprice.Resolution;

                        //for NordPool each ares has prices in 4 currencies, expected records will be *4 for day, hour spot prices
                        int recordsMultiplier = 1;
                        if (areaprice.ExpectedRecords != null)
                        {
                            recordsMultiplier = Convert.ToInt16(areaprice.ExpectedRecords);
                        }

                        int results = QaDao.GetSpotPricesByArea(currentInstrumentArea, reportDate.ToString("yyyy-MM-dd"), currentResolution);
                       // Console.WriteLine("Area = " + currentInstrumentArea + " contains " + results + " records in VizPrices DB");
                        if ((currentResolution.ToUpper() == "DAY") && (results != 1*recordsMultiplier))
                            errorList.Add("Query did not return correct number of spot price records for report date= " + reportDate + ", price area = " 
                                + currentInstrumentArea + ", resolution = " + currentResolution + ".Expected: 1, but was " + results);
                     
                        if (currentResolution.ToUpper() == "HOUR")
                        {
                            if (IsDaylightChanges(reportDate) )
                            {
                                if ((reportDate.Month == 10) && (results != 25* recordsMultiplier))
                                {

                                    errorList.Add(
                                         "Query did not return correct number of spot price records for report date (daylight Saving)= " +
                                        reportDate + ", price area = " + currentInstrumentArea + ", resolution = " +
                                        currentResolution + ".Expected: 25(100), but was " + results);
                                }
                                else
                                {
                                    if (results != 23* recordsMultiplier)
                                        errorList.Add("Query did not return correct number of spot price records for report date (daylight Saving)= " +
                                           reportDate + ", price area = " + currentInstrumentArea + ", resolution = " +
                                           currentResolution + ".Expected: 23(92), but was " + results);
                                }

                            }
                            else
                                if  (results != 24* recordsMultiplier)
                                    errorList.Add("Query did not return correct number of spot price records for report date= " +
                                        reportDate + ", price area = " + currentInstrumentArea + ", resolution = " + 
                                        currentResolution + ".Expected: 24(96), but was " + results);
                        }
                    }
                }

            }
            else throw new ArgumentException("Could not find EUT source by description: " + description);

            foreach (string errorMessage in errorList)
            {
                Console.WriteLine(errorMessage);
            }
            if (errorList.Count > 0) Assert.Fail("Test for source " + description + " failed.");
        }

        public bool IsDaylightChanges(DateTime reportDate)
        {
            DaylightTime dlt = TimeZone.CurrentTimeZone.GetDaylightChanges(reportDate.Year);
            DateTime start = dlt.Start.Date;
            DateTime end = dlt.End.Date;

            if (reportDate == start || reportDate == end)
            {
                return true; 
            }

            return false;
        }

        [Test]
        public void TestLocalFolderJob()
        {
            string destinationPath = @"\\BERSV-FS01\Felles\QA\Regression_EUT\PricesFromLocalFolder\";
            
            string fullTestCaseFilePath = Path.Combine(Directory.GetCurrentDirectory() + localfolderpath);
           
            if (Directory.Exists(destinationPath))
            {
                Directory.Delete(destinationPath, true);
                Thread.Sleep(100);
            }
            try
            {
                DirectoryCopy(fullTestCaseFilePath, destinationPath, true);
            }
            catch (Exception)
            {
                
                throw;
            }
            //Job id for Prices from local folder =35
            int jobid = JobAPI.GetJobsIdByDescription("Prices from local folder", "Historic Data Update Job");
            JobAPI.ExecuteAndAssertJob(jobid, 3600);


        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
