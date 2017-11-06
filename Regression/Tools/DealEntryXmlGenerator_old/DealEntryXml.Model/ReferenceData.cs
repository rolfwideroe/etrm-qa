using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    public class ReferenceData
    {
        public ReferenceData(){}

        public ReferenceData(string comment, string quotaRegion)
        {
            Comment = comment;
            QuotaRegion = quotaRegion;
        }

        //[XmlIgnore]
        public string Comment { get; set; }
        public string QuotaRegion { get; set; }

        //[XmlElement(ElementName = "Comment")]
        //public string XmlComment { get; set; }

        //[XmlIgnore]
        //public bool XmlCommentSpecified { get { return !string.IsNullOrEmpty(this.Comment); } }
    }
}