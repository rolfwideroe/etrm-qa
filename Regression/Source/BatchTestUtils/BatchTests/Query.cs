using System.Xml.Serialization;

namespace ElvizTestUtils.BatchTests
{
    [XmlType("query")]
    public class Query
    {
        [XmlAttribute("file")]
        public string FileName { get; set; }
    }
}