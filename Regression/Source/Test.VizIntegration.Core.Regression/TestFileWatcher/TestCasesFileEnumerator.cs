using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NUnit.Framework;
using System.IO;
using Test.VizIntegration.Core.Regression.Properties;

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
        public static IEnumerable TestCaseFiles
        {
            get
            {
                var processorFileNamePairList = new List<object[]>();
                foreach (KeyValuePair<string, FileProcessor> kvp in TestCaseConfiguration.FileProcessors)
                {

                    if (string.IsNullOrEmpty(kvp.Value.TestFilesFolder))
                        throw new ApplicationException("Folder path property is not set! Cannot fetch files!");
                    var directoryInfo = new DirectoryInfo(kvp.Value.TestFilesFolder);

                    
                    foreach (var fileInfo in directoryInfo.GetFiles())
                    {
                        processorFileNamePairList.Add(new object[] {kvp.Key, fileInfo.Name});
                    }                    
                }

                return processorFileNamePairList.ToArray();
            }
        }
    }
}
