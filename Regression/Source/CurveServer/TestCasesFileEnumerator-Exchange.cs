using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestWCFCurveService
{
    public class TestCasesFileEnumeratorExchange
        {
            /// <summary>
            /// Get files for test cases
            /// </summary>
            public static IEnumerable<string> TestCaseFiles
            {
                get
                {
                    var directoryInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\ExchangeRates"));
                    return directoryInfo.GetFiles().Select(fileInfo => fileInfo.Name);
                }
            }
        }
  

}
