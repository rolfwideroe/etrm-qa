using System.Xml.Serialization;

namespace TestWCFLookupService.TestCases
{
    public class InstrumentCodesTestCase
    {
        public string InsertXml { get; set; }
        public string TestExecutionVenue { get; set; }
        public ExpectedTransaction[] ExpectedResult { get; set; }
    }

    public class ExpectedTransaction
    {
   
        [XmlAttribute]
        public string ExternalId { get; set; }
        public ExpectedCode[] ExpectedCodes { get; set; }
    }

    public class ExpectedCode
    {
        [XmlAttribute]
        public string CodeType { get; set; }
        [XmlAttribute]
        public string Code { get; set; }
    }
}
