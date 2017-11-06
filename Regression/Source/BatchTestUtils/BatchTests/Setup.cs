using System;
using System.Xml.Serialization;

namespace ElvizTestUtils.BatchTests
{
    [XmlType("setup")]
    public class Setup
    {
        [XmlElement("user")]
        public string User { get; set; }
        
        [XmlElement("password")]
        public string Password { get; set; }
        
        [XmlElement("workspace")]
        public string Workspace { get; set; }
        
        [XmlElement("monitor")]
        public string Monitor { get; set; }
        
        [XmlElement("reportdate")]
        public DateTime ReportDate { get; set; }

        [XmlElement("tollerance")]
        public double? Tollerance { get; set; }

        [XmlElement("missingRealizedDataStrategy")]
        public string MissingRealizedDataStrategy { get; set; }

		public ElvizConfiguration[] ElvizConfigurations { get; set; }

		/// <summary>
		/// Controls the ext id(s) in the filter in mutable tests. Should be on format extId1;extId2;extId3
		/// </summary>
		[XmlElement("extId")]
		public string ExtId { get; set; }

		/// <summary>
		/// Controls aggregate structured deals setting in mutable tests. 0 for false, -1 for true.
		/// </summary>
		[XmlElement("aggregateStructuredDeals")]
		public string AggregateStructuredDeals { get; set; }
	}
}