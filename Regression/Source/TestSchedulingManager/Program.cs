using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TestSchedulingManager
{
    static class Program
    {
        private static string expectedOutputFilePath;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args )
        {
            try
            {
                Console.WriteLine("Started tests scheduling manager");
                //check arguments and execute without UI if we have any passed
                const decimal MIN_ARGS = 1;
                    if (args.Length >= MIN_ARGS)
                {
                    RunUnattended(args);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new TestSchedulingManager());
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private static void RunUnattended(IEnumerable<string> args)
        {
            List<string> requiredParameters = new List<string> { "password", "user", "publication" , "database", "path", "expected", "actual"};
            // todo
            //args = new string[]
            //           {
            //               "/pub:New", "/u:vizard", "/p:draziv", "/d:QAECM131",
            //               "/path:C:\\Development\\Elviz\\Development\\Integration\\Viz.Integration.Core.SchedulingModule",
            //               "/expected:C:\\Development\\Elviz\\Development\\Integration\\DealImport\\2012-11-15_New-Expected.xml",
            //               "/actual:C:\\Development\\Elviz\\Development\\Integration\\DealImport"
            //           };
            IList<Parameter> allParameters = new List<Parameter>();
            foreach(string arg in args)
            {
                Parameter parameter = ExtractParameter(arg);
                if (GetParameterValue(allParameters,parameter.Name) == null)
                    allParameters.Add(parameter);
                else 
                    throw new ArgumentException("Parameter" +parameter.Name +" already exists");
            }

            ValidateAllParameters(allParameters, requiredParameters);

            string schedulingManagerPath = GetParameterValue(allParameters, "path");
            expectedOutputFilePath = GetParameterValue(allParameters, "expected");
            string elvizUserName = GetParameterValue(allParameters, "user");
            string elvizPassword = GetParameterValue(allParameters, "password");
            string elvizDatabase = GetParameterValue(allParameters, "database");
            string publicationFileName = GetParameterValue(allParameters, "publication");
            string actualOutputPath = GetParameterValue(allParameters, "actual"); 
            
            FileSystemWatcher fileWatcher = CreateFileWatcher(actualOutputPath);

            try
            {
                string argumentsToSchedulingManager = "/pub:" + publicationFileName + " /user:" + elvizUserName + " /password:" + elvizPassword + " /db:" + elvizDatabase;
                
                 SchedulingManagerExecutor.Run(schedulingManagerPath, argumentsToSchedulingManager);
                 //while (true)
                 //{
                 //    System.Threading.Thread.Sleep(50);
                 //}
                fileWatcher.WaitForChanged(WatcherChangeTypes.Changed, 15000); 
            }

            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private static FileSystemWatcher CreateFileWatcher(string actualOutputPath)
        {
            FileSystemWatcher fileWatcher = new FileSystemWatcher();
            //fileWatcher.Filter = "*.xml";
            //fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Path = actualOutputPath;
            fileWatcher.Changed += FileSystemWatcher1Changed;
            fileWatcher.Created += FileSystemWatcher1Changed;
            return fileWatcher;
        }

        private static string GetParameterValue(IList<Parameter> allParameters, string nameOfParameter)
        {
            foreach (Parameter parameter in allParameters)
            {
                if(parameter.Name == nameOfParameter)
                {
                    return parameter.Value;
                }
            }
            return null;
        }

        private static void ValidateAllParameters(IList<Parameter> allParameters, List<string> requiredParameters)
        {
            IList<string> remainingRequiredParameters = requiredParameters;

            foreach (Parameter parameter in allParameters)
            {
                //if exists in requiredParameters remove it from remaining parameters
                if(remainingRequiredParameters.Contains(parameter.Name))
                {
                    remainingRequiredParameters.Remove(parameter.Name);
                }
                else throw new ArgumentException(parameter.Name +" not required");
                //if it doesnt exist in remaining -> throw exception                
            }
            //if you still have eleemnts in remaining -> throw exception
            if (remainingRequiredParameters.Count != 0) throw new ArgumentException(remainingRequiredParameters[0] + " not supplied");
            foreach (Parameter parameter in allParameters)
            {
                if( parameter.Value == string.Empty ) throw new Exception("Parameter" + parameter.Name+" has no value");
            }
        }

        private static Parameter ExtractParameter(string arg)
        {
            char[] colonArray = new char[] { ':' };
            string[] parts = arg.Split(colonArray);
            string firstPart = parts[0];
            string rest = "";
            string name="";

            if (firstPart.Length < arg.Length)
            {
                rest = arg.Substring(firstPart.Length + 1);
            }

            string value="";
            switch (firstPart.ToLower())
            {
                case "/pub":
                case "/publication":
                    name = "publication";
                    value = rest;
                    break;
                case "/u":
                case "/user":
                    name = "user";
                    value = rest;
                    break;

                case "/p":
                case "/password":
                    name = "password";
                    value = rest;
                    break;

                case "/d":
                case "/db":
                    name = "database";
                    value = rest;
                    break;
                case "/path":
                    name = "path";
                    value = rest;
                    break;
                case "/expected":
                    name = "expected";
                    value = rest;
                    break;
                case "/actual":
                    name = "actual";
                    value = rest;
                    break;
                default: throw new Exception("Unknown token" + firstPart.ToLower());
            }   
  
            return new Parameter(name,value);
        }

        private  static void FileSystemWatcher1Changed(object sender, FileSystemEventArgs e)
        {
            while(FileOpen(e.FullPath))
            {
            }
            string value;
            if(Path.GetExtension(e.FullPath)== ".xml")
                value   = XMLFilesComparer.CompareFiles(e.FullPath, expectedOutputFilePath);
            else if (Path.GetExtension(e.FullPath) == ".csv")
                value = CsvFileComparer.CompareFiles(e.FullPath, expectedOutputFilePath);
            else throw new Exception("Test application supports only xml and csv files");
            
            if(value!=null)
            {
                Console.Write(value);
                Console.WriteLine("Press enter to Exit");
                Console.ReadLine();
            }
        }

        private static bool FileOpen(string fileName)
        {
            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                    return false;
                }
                // The file is not locked
            }
            catch (Exception)
            {
                // The file is locked
            }
            return true;
        }
    }
}
