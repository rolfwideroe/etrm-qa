using System;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace ElvizTestUtils.BatchTests
{
    public class BatchTestFileParser
    {
        public static BatchTestFile DeserializeXml(string testFilepath)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BatchTestFile));

            FileStream readFileStream = File.Open(
                testFilepath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            BatchTestFile batchTestFile;
            try
            {

                batchTestFile = (BatchTestFile)xmlSerializer.Deserialize(readFileStream);

                batchTestFile.Name = Path.GetFileName(testFilepath);

             
                foreach (Assertion assertion in batchTestFile.Assertions)
                {


                  //  string pathBeforeQueryFilePath = Path.GetDirectoryName(testFilepath);

                //    if (pathBeforeQueryFilePath == null) throw new ArgumentException(testFilepath + " has invalid directory");

                    //string queryFileName = assertion.Query.FileName;
                    //string queryPath = Path.Combine(folder, "DBQueries");
                    string queryPath = assertion.Query.FileName;
                    string testFileFolder = Path.GetDirectoryName(testFilepath);
                    string fullPath = Path.Combine(testFileFolder, queryPath);

                    assertion.DbQuery = TestXmlTool.Deserialize<DbQuery>(fullPath);
                    string sqlQuery = assertion.DbQuery.SqlQuery;
                    assertion.Name = Path.GetFileName(fullPath);


                    string preparedSqlQuery =
                        sqlQuery.Replace("{reportdate}", batchTestFile.Setup.ReportDate.ToString("yyyy-MM-dd"))
                            .Replace("{user}", batchTestFile.Setup.User)
                            .Replace("{workspace}", batchTestFile.Setup.Workspace)
                            .Replace("{monitor}", batchTestFile.Setup.Monitor)
                            ;


                    assertion.DbQuery.PreparedSqlQuery = preparedSqlQuery;


                }
            }
            finally
            {
                readFileStream.Close();
            }

            return batchTestFile;
        }

        //public static BatchTestFile DeserializeXml(string testFilepath)
        //{
        //    string pathBeforeQueryFilePath = Path.GetDirectoryName(testFilepath);
        //    return DeserializeXml(testFilepath, pathBeforeQueryFilePath);
        //}

    
    }

   
}
