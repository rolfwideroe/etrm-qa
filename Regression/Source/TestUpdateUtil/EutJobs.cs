using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;

namespace TestElvizUpdateTool
{
    public sealed class EutJobs
    {
        public EutJobs(string testCaseName)
        {
            TestCaseName = testCaseName;
            ExecutingAssembly = Assembly.GetExecutingAssembly();
            LocalPath = new Uri(ExecutingAssembly.CodeBase).LocalPath;
            DirectoryName = Path.GetDirectoryName(LocalPath);
            Environment.CurrentDirectory = DirectoryName ?? throw new InvalidOperationException($"Path {LocalPath} does not exist");
            TestCaseFile = Path.Combine(DirectoryName, TestCaseName);
            TestCaseJobsList = TestXmlTool.Deserialize<JobsTestCase>(TestCaseFile);
        }

        private static string TestCaseName { get; set; }
        private Assembly ExecutingAssembly { get; }
        private string LocalPath { get; }
        private string DirectoryName { get; }
        private string TestCaseFile { get; }
        private JobsTestCase TestCaseJobsList { get; }
        public IEnumerable<JobItem> TestCaseJobItemsList => TestCaseJobsList.JobItems.ToList();
    }
}
