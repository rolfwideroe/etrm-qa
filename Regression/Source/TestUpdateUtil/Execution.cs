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

        static List<MessageDetails> MessageDetailsList { get; set; }

        public bool JobExecuted()
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            AddMessage($"JobId : {JobId}", callingClass, $"Description : {Description}");

            if (ExecutedJobStatus() == "Success") return Status;
            if (ExecutedJobStatus() == "Success") return Status;

            return JobAPI.ExecuteAndAssertJob(JobId, null, 800) > 0;
        }

        private string ExecutedJobStatus()
        {
            return JobAPI.ExecuteJob(JobId, null, 800).Status;
        }

        private static void AddMessage(string fileName, Type callingClass, string filePath)
        {
            MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Debug, callingClass,
                GetCurrentMethodName(), $"{fileName} ----  {filePath}",
                "Debugging Reg Test Failures"));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static string GetCurrentMethodName()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);

            return stackFrame.GetMethod().Name;
        }
    }
}
