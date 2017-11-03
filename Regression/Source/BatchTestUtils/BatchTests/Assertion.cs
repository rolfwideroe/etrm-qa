using System.Data;
using System.Xml.Serialization;
using ElvizTestUtils.ReportEngineTests;

namespace ElvizTestUtils.BatchTests
{
    [XmlType("datawarehouse")]
    public class Assertion
    {
        private DataTable expectedDataTable;

        [XmlElement("query")]
        public Query Query { get; set; }
        
        [XmlArray("returns")]
        [XmlArrayItem("record")]
        public string[] ExpectedRecords { get; set; }

        public DbQuery DbQuery { get; set; }

        public string Name { get; set; }

        public DataTable ExpectedDataTable
        {
            get
            {
                if (DbQuery == null) return null;

                if (expectedDataTable == null)
                {
                    const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
                    expectedDataTable = DataTableHelper.CreateDataTableFromExpectedRecords(this.Name, this.DbQuery.Columns, this.ExpectedRecords,dateTimeFormat);
                }
                return expectedDataTable;
            }
        }
    }
}