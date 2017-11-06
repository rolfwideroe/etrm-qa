using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ElvizTestUtils
{

    public class JobsTestCase
    {
        public JobItem[] JobItems { get; set; }
    }

    public class JobItem
    {
        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("JobId")]
        public int JobId { get; set; }

        [XmlAttribute("ExecutionVenue")]
        public string ExecutionVenue { get; set; }

        public SpotPrice[] SpotPrices { get; set; }
        public InstrumentPrice[] InstrumentPrices { get; set; }
    }

    public class SpotPrice
    {
        [XmlAttribute("Area")]
        public string Area { get; set; }

        [XmlAttribute("Resolution")]
        public string Resolution { get; set; }

        [XmlAttribute("ExpectedRecords")]
        public string ExpectedRecords { get; set; }

    }
    public class InstrumentPrice
    {
        [XmlAttribute("Area")]
        public string Area { get; set; }

        [XmlAttribute("CfdArea")]
        public string CfdArea { get; set; }

    }

}
