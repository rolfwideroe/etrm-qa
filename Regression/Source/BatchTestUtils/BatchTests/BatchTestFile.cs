using System.Xml.Serialization;

namespace ElvizTestUtils.BatchTests
{
    [XmlRoot("test")]
    public class BatchTestFile
    {
        [XmlIgnore]
        public string Name { get; set; }
        
        [XmlElement("setup")]
        public Setup Setup { get; set; }

        [XmlArray("assert")]
        public Assertion[] Assertions { get; set; }
    }
}