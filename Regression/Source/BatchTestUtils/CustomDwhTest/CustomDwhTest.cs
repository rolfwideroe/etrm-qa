using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.DatabaseTools;

namespace ElvizTestUtils.CustomDwhTest
{
    public class CustomDwhTest
    {
        public InputData InputData { get; set; }
        public Monitor[] Assert { get; set; }
    }

    public class InputData
    {
        public string EtrmUser { get; set; }
        public string EtrmPassword { get; set; }
        public string Workspace { get; set; }
        public DateTime ReportDate { get; set; }
        public string SqlInstance { get; set; }
        public string SqlUser { get; set; }
        public string SqlPassword { get; set; }

        public string ExpectedDatawareHouse { get; set; }
        public string ActualDatawareHouse { get; set; }
    }

    public class Monitor
    {
        [XmlAttribute]
        public string Name { get; set; }

        public string QueryFilePath { get; set; }

    

        public DataTable GetExpectedDataTable(InputData inputData,DbQuery query )
        {
            return GetDataTable(inputData, query, inputData.ExpectedDatawareHouse);
        }

        public DataTable GetActualDataTable(InputData inputData, DbQuery query)
        {
            return GetDataTable(inputData, query, inputData.ActualDatawareHouse);
        }

        private DataTable GetDataTable(InputData inputData, DbQuery query ,string datawareHouse)
        {
            string monitorName = this.Name;

            string preparedSqlQuery = query.SqlQuery.Replace("{reportdate}", inputData.ReportDate.ToString("yyyy-MM-dd"))
                .Replace("{workspace}", inputData.Workspace)
                .Replace("{monitor}", monitorName);

            Console.WriteLine(preparedSqlQuery);

            DataTable expecteDataTable = QaDao.DataTableFromSql(inputData.SqlInstance,datawareHouse, inputData.SqlUser, inputData.SqlPassword, preparedSqlQuery);
            expecteDataTable.TableName = monitorName;

            return expecteDataTable;

        }
    }

    public class Expected
    {
        public string DatawareHouse { get; set; }
    }

    public class Actual
    {
        public string DatawareHouse { get; set; }
    }
}
