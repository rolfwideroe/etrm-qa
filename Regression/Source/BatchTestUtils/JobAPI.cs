using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.InternalJobService;
using NUnit.Framework;

namespace ElvizTestUtils
{
    public class JobAPI
    {
        [Test]
        public static int GetJobsIdByDescription(string description, string jobType)
        {
            IJobService service = WCFClientUtil.GetJobServiceClient();

            JobCriteria curveJob = new JobCriteria
            {
                Description = description,
                JobType = jobType
            };

            Job[] jobs = service.QueryJobs(curveJob);
            foreach (Job job in jobs)
            {
                if (job.Description == description && job.JobType == jobType)
                    return job.Id;
            }
            throw new ArgumentException("Could not find job by description: "+description);
            
        }

        public static int GetJobsIdByDescription(string description, string jobType, string appServerName)
        {
            IJobService service = WCFClientUtil.GetJobServiceClient(appServerName);

            JobCriteria curveJob = new JobCriteria
            {
                Description = description,
                JobType = jobType
            };

            Job[] jobs = service.QueryJobs(curveJob);
            foreach (Job job in jobs)
            {
                if (job.Description == description && job.JobType == jobType)
                    return job.Id;
            }
            throw new ArgumentException("Could not find job by description: " + description);

        }

        public static IEnumerable<Job> GetJobsByJobtype(string jobType,string appServerName)
        {
            IJobService service = WCFClientUtil.GetJobServiceClient(appServerName);

            JobCriteria curveJob = new JobCriteria
            {
                JobType = jobType
            };

            Job[] jobs = service.QueryJobs(curveJob);

            if (jobs.Length > 0)
                return jobs;

            throw new ArgumentException("Could not find any job with job type= " + jobType);
        }

        private static JobServiceClient GetJobServiceClient(string appServerName)
        {

            if (appServerName == null) return WCFClientUtil.GetJobServiceClient();
            return WCFClientUtil.GetJobServiceClient(appServerName);
        }

        public static Job[] GetJobsByDescriptionOrJobtype(string description, string jobType = null,string appServerName =null)
        {
            IJobService service = GetJobServiceClient(appServerName);

            JobCriteria curveJob = new JobCriteria
            {
                Description = description,
                JobType = jobType
            };

            Job[] jobs = service.QueryJobs(curveJob);

            if (jobs.Length > 0)
                return jobs;

            throw new ArgumentException("Could not find any job by description = " + description + " and job type= " + jobType);

        }

        public static Job FindJob(string description, string jobType, string appServerName = null)
        {
            IJobService service = GetJobServiceClient(appServerName);

            JobCriteria criteria = new JobCriteria
            {
                Description = description,
                JobType = jobType
            };

            Job[] jobs = service.QueryJobs(criteria);

            if (jobs.Length == 0)
                return null;

            if (jobs.Length ==1)
                return jobs[0];

            throw new ArgumentException("Found more than one Job with description = " + description + " and job type= " + jobType);

        }


        public static int ExecuteAndAssertJob(int jobId, int statusChangeTimeoutSeconds)
        {
           return ExecuteAndAssertJob(jobId, new Dictionary<string, string>(), statusChangeTimeoutSeconds);
        }

        public static string GetJobExecutionLog(int jobExecutionId, string appServerName = null)
        {
            JobServiceClient client = GetJobServiceClient(appServerName);

            LogEntry[] errorLog = client.QueryJobExecutionLog(new JobLogCriteria() { ExecutionId = jobExecutionId});

            string log = "";

            foreach (LogEntry logEntry in errorLog)
            {
                log +=logEntry.Severity + "\t" + logEntry.Source +Environment.NewLine+ "\t\t" +
                       logEntry.Message + Environment.NewLine+Environment.NewLine;
            }
            return log;
        }

        public static JobExecutionStatus ExecuteJob(int jobId, Dictionary<string, string> optionalParams,int statusChangeTimeoutSeconds, string appServerName = null)
        {
            JobServiceClient client = GetJobServiceClient(appServerName);

            JobExecutionRequest request = new JobExecutionRequest
            {
                JobId = jobId,
                OptionalParameters = optionalParams
            };

            Console.WriteLine("job started " + DateTime.Now);
            JobExecutionStatus status = client.SubmitJobExecution(request);
            int executionId = status.ExecutionId;
            JobStatusRequest jobStatusRequest = new JobStatusRequest {ExecutionId = executionId};

            if(statusChangeTimeoutSeconds<6000)
                return client.WaitForExecutionStatusChange(jobStatusRequest, statusChangeTimeoutSeconds);

            int cycleInSeconds = 600;
            int cyclesToWait = statusChangeTimeoutSeconds/cycleInSeconds + 1;

            for (int i = 0; i < cyclesToWait; i++)
            {
                JobExecutionStatus cycleStatus= client.WaitForExecutionStatusChange(jobStatusRequest, cycleInSeconds);

                if (cycleStatus != null) return cycleStatus;
            }

            return null;
        }

        public static int ExecuteAndAssertJob(int jobId, Dictionary<string, string> optionalParams, int statusChangeTimeoutSeconds,string appServerName =null)
        {
            JobExecutionStatus finalStatus = ExecuteJob(jobId, optionalParams, statusChangeTimeoutSeconds, appServerName);

            if(finalStatus==null) throw new ArgumentException("JobId "+jobId+" did not complete within the set timeout of :"+ statusChangeTimeoutSeconds + " secconds");
            JobServiceClient client = GetJobServiceClient(appServerName);

            Console.WriteLine("job ended "+DateTime.Now);
            if (finalStatus.Status != "Success")
            {
                IList<string> errorAndDebugLog = new List<string>();

                LogEntry[] errorLog = client.QueryJobExecutionLog(new JobLogCriteria() { ExecutionId = finalStatus.ExecutionId, Severity = "Error" });

                foreach (LogEntry logEntry in errorLog)
                {
                    errorAndDebugLog.Add(logEntry.Message);
                }

                LogEntry[] debugLog = client.QueryJobExecutionLog(new JobLogCriteria() { ExecutionId = finalStatus.ExecutionId, Severity = "Debug" });

                foreach (LogEntry logEntry in debugLog)
                {
                    errorAndDebugLog.Add(logEntry.Message);
                }

                string failMessage = "";

                foreach (string s in errorAndDebugLog)
                {
                    failMessage += s + '\n';
                }

                Assert.Fail(failMessage);
            }

            return finalStatus.ExecutionId;

        }

        //for EUT jobs
        public static void RunEutJobOncePerDay(string description, string jobType = "Historic Data Update Job")
        {
            //string description = "EEX (power and gas)";
            Job[] eutJobs = GetJobsByDescriptionOrJobtype(description, jobType);

            Assert.AreEqual(1, eutJobs.Length, "Function returned more that one job corresponding specified description/jobtype " + description + "/" + jobType);
            if (eutJobs[0].LastRunResult == "Success")
            {
                DateTime lastRunTime = eutJobs[0].LastRunTimeUtc;
                DateTime localLastRunTime = lastRunTime.ToLocalTime();
                int today = DateTime.Today.Day;
                if (today > localLastRunTime.Day)
                    ExecuteAndAssertJob(eutJobs[0].Id, 600);
            }
            else
                ExecuteAndAssertJob(eutJobs[0].Id, 600);
       
        }
        //added extra function to avoid test to fail because of problem with EEX source when downloading prices 
        //which is not related to current test
        public static void RunEutJobOncePerDayWithoutAssert(string description, string jobType = "Historic Data Update Job")
        {
            //string description = "EEX (power and gas)";
            Job[] eutJobs = GetJobsByDescriptionOrJobtype(description, jobType);

            Assert.AreEqual(1, eutJobs.Length, "Function returned more that one job corresponding specified description/jobtype " + description + "/" + jobType);
            if (eutJobs[0].LastRunResult == "Success")
            {
                DateTime lastRunTime = eutJobs[0].LastRunTimeUtc;
                DateTime localLastRunTime = lastRunTime.ToLocalTime();
                int today = DateTime.Today.Day;
                if (today > localLastRunTime.Day)
                    ExecuteJob(eutJobs[0].Id, null, 600);
            }
            else
                ExecuteJob(eutJobs[0].Id, null, 600);

        }


    }
}
