using ElvizTestUtils;
using MessageHandler;
using MessageHandler.Pocos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestElvizUpdateTool
{
    internal class Execution
    {
        internal Execution(string description)
        {
            Description = description;
            JobId = JobAPI.GetJobsIdByDescription(Description, "Historic Data Update Job");

        }
        string Description { get; }
        bool Status { get; set; } = true;
        int JobId { get; set; }

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
