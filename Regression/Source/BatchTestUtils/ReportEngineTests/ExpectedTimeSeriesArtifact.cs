using System.Data;
using System.Xml.Serialization;

namespace ElvizTestUtils.ReportEngineTests
{
    public class ExpectedTimeSeriesArtifact
    {
        private DataTable dataTable;

        [XmlAttribute]
        public string ArtifactId { get; set; }

        public TestProperties[] TimeSeriesProperties { get; set; }

        [XmlAttribute]
        public string Columns { get; set; }

        [XmlAttribute]
        public string ColumnsDataTypes { get; set; }

        [XmlElement]
        public string[] ExpectedRecord { get; set; }


        public DataTable DataTable
        {
            get
            {
                if (dataTable == null)
                {
                    const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
                    dataTable = ReportEngineTestHelper.CreateDataTableFromExpectedRecords(this.ArtifactId, Columns, ColumnsDataTypes, ExpectedRecord,dateTimeFormat);
                }

                return dataTable;
            }
        }
    }
}
