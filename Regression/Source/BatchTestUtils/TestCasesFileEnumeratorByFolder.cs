using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ElvizTestUtils
{
    public class TestCasesFileEnumeratorByFolder
    {
        /// <summary>
        /// Get files for test cases
        /// </summary>
        public static IEnumerable<string> TestCaseFiles(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var localPath = new Uri(assembly.CodeBase).LocalPath;
            var directoryName = Path.GetDirectoryName(localPath);
            if (directoryName != null)
            {
                Environment.CurrentDirectory = directoryName;

                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(directoryName, path));
                return directoryInfo.GetFiles().Select(fileInfo => fileInfo.Name).OrderBy(x => x);
            }
            throw new ArgumentException("TestCasesFileEnumeratorByFolder. Local path directory for current assembly is null.");
        }

        public static IEnumerable<string> TestCaseFilesFiltred(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var localPath = new Uri(assembly.CodeBase).LocalPath;
            var directoryName = Path.GetDirectoryName(localPath);
            if (directoryName != null)
            {
                Environment.CurrentDirectory = directoryName;

                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(directoryName, path));
            
                return  directoryInfo.GetFiles().Where(fileinfo => fileinfo.Name.StartsWith("Pass") && !fileinfo.Name.Contains("Bulk") && !fileinfo.Name.Contains("Extends") && !fileinfo.Name.Contains("Extends_")
                                                                && !fileinfo.Name.Contains("Update")
                                                                && !fileinfo.Name.Contains("empty")
                                                                && !fileinfo.Name.Contains("Gencon")).Select(fileInfo => fileInfo.Name).OrderBy(x => x);
            }
            throw new ArgumentException("TestCasesFileEnumeratorByFolder. Local path directory for current assembly is null.");
        }

        public static IEnumerable<string> TestCaseFilesFiltredContains(string path,IDictionary<string,bool> containsIncludeExclude )
        {
            var assembly = Assembly.GetExecutingAssembly();
            var localPath = new Uri(assembly.CodeBase).LocalPath;
            var directoryName = Path.GetDirectoryName(localPath);
            if (directoryName != null)
            {
                Environment.CurrentDirectory = directoryName;


                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(directoryName, path));

                IEnumerable<string> files = directoryInfo.GetFiles().Select(fileInfo => fileInfo.Name);

                foreach (KeyValuePair<string, bool> conPair in containsIncludeExclude)
                {
                    if (conPair.Value)
                        files = files.Where(x => x.ToUpper().Contains(conPair.Key.ToUpper()));
                    else
                        files = files.Where(x => !x.ToUpper().Contains(conPair.Key.ToUpper()));


                }

                return files.OrderBy(x => x);
            }
            throw new ArgumentException("TestCasesFileEnumeratorByFolder. Local path directory for current assembly is null.");
            //return directoryInfo.GetFiles().Where(fileinfo => fileinfo.Name.ToUpper().Contains(contains.ToUpper())).Select(fileInfo => fileInfo.Name).OrderBy(x => x);

        }

        public static IEnumerable<string> TestCaseFilesFiltredEndsWith(string path, string criteria)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var localPath = new Uri(assembly.CodeBase).LocalPath;
            var directoryName = Path.GetDirectoryName(localPath);
            if (directoryName != null)
            {
                Environment.CurrentDirectory = directoryName;

                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(directoryName, path));

                IEnumerable<string> files = directoryInfo.GetFiles().Select(fileInfo => fileInfo.Name);

                files = files.Where(x => x.EndsWith(criteria, StringComparison.CurrentCultureIgnoreCase));

                return files.OrderBy(x => x);
            }
            throw new ArgumentException("TestCasesFileEnumeratorByFolder. Local path directory for current assembly is null.");
        }

    }
}
