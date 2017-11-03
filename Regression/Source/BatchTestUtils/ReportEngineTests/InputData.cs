using System.Xml.Serialization;

namespace ElvizTestUtils.ReportEngineTests
{
    public class InputData
    {
        public string ReportId { get; set; }
        public double Tolerance { get; set; }
        public TestSetting[] Settings { get; set; }
        public ElvizConfiguration[] ElvizConfigurations { get; set; }
    }

    [XmlType("Setting")]
    public class TestSetting
    {
        [XmlAttribute]
        public string SettingId { get; set; }
        [XmlAttribute]
        public bool IsNotSet { get; set; }
        [XmlAttribute]
        public string XmlEncodedValue { get; set; }
    }

    [XmlType("Property")]
    public class TestProperties
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string DataType { get; set; }
        [XmlIgnore]
        public string Value { get; set; }
    }
}