using System.Xml.Serialization;

namespace ElvizTestUtils
{
    public class ElvizConfiguration
    {
        public ElvizConfiguration()
        {
        }

        public ElvizConfiguration(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public ElvizConfiguration(string name, string value, string defaultValue)
        {
            Name = name;
            Value = value;
            DefaultValue = defaultValue;
        }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlIgnore]
        public string DefaultValue { get; set; }

    }
}
