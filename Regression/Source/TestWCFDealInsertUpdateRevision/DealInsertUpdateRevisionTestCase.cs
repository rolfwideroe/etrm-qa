using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TestWCFDealInsertUpdateRevision
{
 
       
        //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
        //[Serializable()]
        //[System.Diagnostics.DebuggerStepThroughAttribute()]
        //[System.ComponentModel.DesignerCategoryAttribute("code")]
        //[XmlType(AnonymousType = true)]
        //[XmlRoot(Namespace = "", IsNullable = false)]
        public class ExpectedTimeSeriesSet
        {
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

        public class ExpectedTimeSeriesValue //: IXmlSerializable
    {
        [XmlAttribute("FromDateTime")]
        public DateTime FromDateTime { get; set; }

        [XmlAttribute("UntilDateTime")]
        public DateTime UntilDateTime { get; set; }

        [XmlAttribute("UtcFromDateTime")]
        public DateTime UtcFromDateTime { get; set; }

        [XmlAttribute("UtcUntilDateTime")]
        public DateTime UtcUntilDateTime { get; set; }

        [XmlAttribute("Value")]
        public double Value { get; set; }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            double value;

            if (double.TryParse(reader.GetAttribute("Value"), out value))
            {
                Value = value;
            }

            
        }

        public void WriteXml(XmlWriter writer)
        {
            
        }
    }

 


    
}
