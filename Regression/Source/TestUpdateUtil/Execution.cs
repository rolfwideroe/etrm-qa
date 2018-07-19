using ElvizTestUtils;

namespace TestElvizUpdateTool
{
    internal class Execution
    {
        internal Execution(string description)
        {
            Description = description;
            JobId  = JobAPI.GetJobsIdByDescription(Description, "Historic Data Update Job");
        }
        private string Description { get; }
        private bool Status { get; set; } = true;
        private int JobId { get; set; }

        public bool JobExecuted()
        {
            if (ExecutedJobStatus() == "Success") return Status;
            if (ExecutedJobStatus() == "Success") return Status;

           return JobAPI.ExecuteAndAssertJob(JobId, null, 800) > 0;
        }

        private string ExecutedJobStatus()
        {
            return JobAPI.ExecuteJob(JobId, null, 800).Status;
        }
    }
}
