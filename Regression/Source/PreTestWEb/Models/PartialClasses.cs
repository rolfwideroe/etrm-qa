using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PreTestWeb.Models
{
    public partial class Workspace
    {
        public string LastTestResult
        {
            get
            {
                TestEnvRun lastTestEnvRun = this.TestEnvRuns.LastOrDefault();
                if (lastTestEnvRun != null)
                {
                    IEnumerable<TestResult> results = lastTestEnvRun.TestResults;

                    if (results == null || !results.Any()) return null;

                    int totalResults = results.Count();

                    int failures = results.Count(x => x.TestResultStatus != "Success");

                    if (totalResults == failures) return "All Failed";
                    if ( failures==0) return "All Passsed";
                    if (failures>0 ) return "Some Failed";

                    return "Unknown";

                }
                return null;
            }
        }
    }
}