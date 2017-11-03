using System.Data;
using System.Xml.Serialization;

namespace ElvizTestUtils.ReportEngineTests
{
    public class ExpectedDataTableArtifact
    {
        private DataTable dataTable;
	    private int transactionIdColumnIndex = -1;

	    [XmlAttribute]
	    public int TransactionIdColumnIndex
	    {
		    get { return transactionIdColumnIndex; }
		    set { transactionIdColumnIndex = value; }
	    }

	    [XmlAttribute]
        public string ArtifactId { get; set; }

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
