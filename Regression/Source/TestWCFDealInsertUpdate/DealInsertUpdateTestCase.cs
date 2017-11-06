using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TestWCFDealInsertUpdate
{


    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class DealInsertUpdateTestCase
    {
        public string InsertXml { get; set; }
        public string UpdateXml { get; set; }
        public UpdateExcel UpdateExcel { get; set; }
        public ReportDate ReportDate { get; set; }
        public ExpectedResult ExpectedResult { get; set; }
    }

    public class UpdateExcel
    {
        public string FileName { get; set; }
    }

    public class ReportDate
    {
        [XmlAttribute]
        public DateTime Value { get; set; }
    }

    public class ExpectedResult
    {
        [XmlAttribute]
        public string ExpectedType { get; set; }

        public string ErrorMessage { get; set; }

        public ExpectedTimeSeriesSet[] ExpectedTimeSeriesSets { get; set; }
        
        [XmlElement("ExpectedVersions")]
        public ExpectedVersions[] ExpectedVersions { get; set; }
       
    }
    
       //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
        //[Serializable()]
        //[System.Diagnostics.DebuggerStepThroughAttribute()]
        //[System.ComponentModel.DesignerCategoryAttribute("code")]
        //[XmlType(AnonymousType = true)]
        //[XmlRoot(Namespace = "", IsNullable = false)]
        public class ExpectedTimeSeriesSet
        {
            private string modelType;
            public ExpectedTimeSeriesSet()
            {
                modelType = "";
            }

            [XmlAttribute]
            public string ExternalId { get; set; }

            [XmlElement(ElementName = "ModelType",IsNullable = true)]
            public string ModelType 
            { 
                get { return modelType.Length == 0 ? "Retail" : modelType; } 
                set { modelType = value; } 
            }

            [XmlElement("ExpectedTimeSeries")]
            public ExpectedTimeSeries[] ExpectedTimeSeries { get; set; }
        }
        
   
        public class ExpectedTimeSeries
        {
            [XmlElement("Resolution", typeof(string))]
            public string Resolution { get; set; }

            [XmlElement("TimeSeriesTypeName", typeof(string))]
            public string TimeSeriesTypeName { get; set; }

            [XmlElement("TimezoneName", typeof(string))]
            public string TimezoneName { get; set; }
                         
            [XmlArray("ExpectedTimeSeriesValues")]
            public ExpectedTimeSeriesValue[] ExpectedTimeSeriesValues{ get; set; }
        }

        public class ExpectedTimeSeriesValue : IXmlSerializable
        {

            //[XmlAttribute("FromDateTime")]
            public DateTime FromDateTime { get; set; }

            //[XmlAttribute("UntilDateTime")]
            public DateTime UntilDateTime { get; set; }

            //[XmlAttribute("UtcFromDateTime")]
            public DateTime UtcFromDateTime { get; set; }

            //[XmlAttribute("UtcUntilDateTime")]
            public DateTime UtcUntilDateTime { get; set; }

            //[XmlAttribute("Value")]
            public double? Value { get; set; }

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                FromDateTime = ReadDateTimeAttribute(reader, "FromDateTime");
                UntilDateTime = ReadDateTimeAttribute(reader, "UntilDateTime");
                UtcFromDateTime = ReadDateTimeAttribute(reader, "UtcFromDateTime");
                UtcUntilDateTime = ReadDateTimeAttribute(reader, "UtcUntilDateTime");
                Value = ReadNullableDoubleAttribute(reader, "Value");
                reader.Read();
            }

            private static DateTime ReadDateTimeAttribute(XmlReader reader, string attributeName)
            {
                DateTime dateTime;

                DateTimeStyles dateTimeStyles = DateTimeStyles.AdjustToUniversal;
                
                
                DateTime.TryParse(reader.GetAttribute(attributeName),new DateTimeFormatInfo(), dateTimeStyles,out dateTime);

                return dateTime;
            }

            private static double? ReadNullableDoubleAttribute(XmlReader reader, string attributeName)
            {
                double val;
                double? result = null;

                if (reader.GetAttribute(attributeName) != null)
                {
                    if (double.TryParse(reader.GetAttribute(attributeName), out val))
                    {
                        result = val;
                    }
                }
                
                return result;
            }

            public void WriteXml(XmlWriter writer)
            {
                //throw new NotImplementedException();

                writer.WriteAttributeString("FromDateTime", FromDateTime.ToString("s"));
                writer.WriteAttributeString("UntilDateTime", UntilDateTime.ToString("s"));
                writer.WriteAttributeString("UtcFromDateTime", UtcFromDateTime.ToString("u"));
                writer.WriteAttributeString("UtcUntilDateTime", UtcUntilDateTime.ToString("u"));
                if (Value.HasValue)
                    writer.WriteAttributeString("Value", Value.ToString());
            }
    }

        public class ExpectedVersionsSet
        {
            [XmlElement("ExpectedVersions")]
            public ExpectedVersions[] ExpectedVersions { get; set; }
        }


        public class ExpectedVersions
        {
            [XmlElement("ExpectedVersion")]
            public ExpectedVersion[] ExpectedVersion { get; set; }

            [XmlAttribute("ExternalId")]
            public string ExternalId { get; set; }
        }

    
        public class ExpectedVersion
        {
            [XmlAttribute("IsLatestVersion")]
            public bool IsLatestVersion { get; set; }

            [XmlElement("EffectiveDate")]
            public DateTime EffectiveDate { get; set; }

            [XmlElement("ExpectedVersionDate")]
            public DateTime ExpectedVersionDate { get; set; }
        
            [XmlElement(ElementName = "ExpectedTimeSeriesSet", IsNullable = true)]
            public ExpectedTimeSeriesSet ExpectedTimeSeriesSet { get; set; }
        }
}
