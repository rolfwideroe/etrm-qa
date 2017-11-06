using System;
using System.Xml.Serialization;

namespace ElvizTestUtils.ReportingDbTimeSeries
{
    public class ReportingDBTestCase
    {
        public InputData InputData { get; set; }
        public ExpectedValues ExpectedValues { get; set; }
    }

    public class InputData
    {
        public string JobName { get; set; }
        public string JobType { get; set; }
        public DateTime ReportDate { get; set; }

        public int? TimeOutInSeconds { get; set; }
        public bool ShouldSerializeTimeOutInSeconds() { return TimeOutInSeconds != null; }

        public string MissingRealizedDataStrategy { get; set; }

        public JobParameter[] JobParameters { get; set; }
        public ElvizConfiguration[] ElvizConfigurations { get; set; }
    }

    public class JobParameter
    {
        [XmlAttribute]
        public string ParameterName { get; set; }
        [XmlAttribute]
        public string ParameterValue { get; set; }
    }

    public class ExpectedValues
    {
        public ExpectedDataTable ExpectedDataTable { get; set; }
    }

    public class ExpectedDataTable
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlIgnore]
        public string DataType {
            get
            {
                if (string.IsNullOrEmpty(this.Type))
                    return "TimeSeries";
                return this.Type;
            } }

        [XmlElement]
        public string[] ExpectedRecord { get; set; }

    }
}
