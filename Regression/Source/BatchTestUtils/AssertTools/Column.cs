using System.Xml.Serialization;

namespace ElvizTestUtils.AssertTools
{
    public class Column
    {
        [XmlAttribute]
        public string ColumnName { get; set; }

        [XmlAttribute]
        public string ColumnDataType { get; set; }
    }
}