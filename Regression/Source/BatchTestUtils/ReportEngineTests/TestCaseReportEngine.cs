using System;
using System.Xml.Serialization;

namespace ElvizTestUtils.ReportEngineTests
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class TestCaseReportEngine
    {
        public InputData InputData { get; set; }
        public ExpectedValues ExpectedValues { get; set; }
    }

    public class ExpectedValues
    {
        public string ExceptionErrorMessage { get; set; }

        public ExpectedDataTableArtifact[] ExpectedDataTableArtifacts { get; set; }
        public ExpectedTimeSeriesArtifact[] ExpectedTimeSeriesArtifacts { get; set; }
    }
}
