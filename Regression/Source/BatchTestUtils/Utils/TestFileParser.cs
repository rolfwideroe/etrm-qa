using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EcmBatchTests.Utils
{
    internal class TestFileParser
    {
        public string TestType { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string Workspace { get; private set; }

        public string Monitor { get; private set; }

        public string ReportDate { get; private set; }

        public Dictionary<string, string[]> Assertions { get; private set; }

        public TestFileParser(string fileName)
        {
            var xmlDoc = XDocument.Load(fileName);

            try
            {
                TestType = xmlDoc.Root.Attribute("type").ToString();
                Name = xmlDoc.Root.Attribute("name").ToString();
                Description = xmlDoc.Root.Attribute("description").ToString();

                UserName = xmlDoc.Root.Element("setup").Element("user").Value;
                Password = xmlDoc.Root.Element("setup").Element("password").Value;
                Workspace = xmlDoc.Root.Element("setup").Element("workspace").Value;
                Monitor = xmlDoc.Root.Element("setup").Element("monitor").Value;
                ReportDate = xmlDoc.Root.Element("setup").Element("reportdate").Value;

                var assertions = new Dictionary<string, string[]>();

                foreach (var assertion in xmlDoc.Root.Element("assert").Elements("datawarehouse"))
                {
                    var sqlQuery = assertion.Element("query").Value;
                    if (assertion.Element("query").Attribute("file") != null)
                    {
                        sqlQuery = File.ReadAllText(assertion.Element("query").Attribute("file").Value);
                    }

                    sqlQuery = PrepareQuery(sqlQuery);

                    assertions[sqlQuery] =
                        (from r in assertion.Element("returns").Elements("record") select r.Value).ToArray();
                }

                Assertions = assertions;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("File {0} does not seem to be a correct test file.", fileName), e);
            }
        }

        string PrepareQuery(string query)
        {
            return query.Replace("{reportdate}", ReportDate).Replace("{user}", UserName).Replace("{workspace}", Workspace).Replace("{monitor}", Monitor);
        }
    }
}
