using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Test.VizIntegration.Core.Regression
{
    /// <summary>
    /// Enumerates all files in the folder
    /// </summary>
    public class TestCasesFileEnumerator
    {
        /// <summary>
        /// Get files for test cases
        /// </summary>
        public static IEnumerable<string> TestCaseFiles
        {
            get
            {                
                var directoryInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles"));
                return directoryInfo.GetFiles().Select(fileInfo => fileInfo.FullName);
            }
        }
    }
}
