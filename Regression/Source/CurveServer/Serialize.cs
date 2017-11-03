using System;
using System.Xml.Serialization;

namespace TestWCFCurveService
{
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class TestCase
    {
            public InputData InputData { get; set; }
            public ExpectedValue[] ExpectedValues { get; set; }
    }

    public class ExpectedValue
    {
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }
        [XmlAttribute("Value")]
        public double Value { get; set; }
        [XmlText()]
        public string Text { get; set; }
    }

    public class InputData
    {
        private string currencyISOCode;
        private string baseCurrencyISOCode;
        private string crossCurrencyCode;
        private string currencySource;
        private DateTime reportDate;
        private DateTime lastDate;
        private string resolution;


        [XmlElement("currencyISOCode", typeof(string))]
        public string CurrencyISOCode
        {
            get { return this.currencyISOCode; }
            set { this.currencyISOCode = value; }
        }

        [XmlElement("baseCurrencyISOCode", typeof(string))]
        public string BaseCurrencyISOCode
        {
            get { return this.baseCurrencyISOCode; }
            set { this.baseCurrencyISOCode = value; }
        }

        [XmlElement("crossCurrencyCode", typeof(string))]
        public string CrossCurrencyCode
        {
            get { return this.crossCurrencyCode; }
            set { this.crossCurrencyCode = value; }
        }

        [XmlElement("currencySource", typeof(string))]
        public string CurrencySource
        {
            get { return this.currencySource; }
            set { this.currencySource = value; }
        }

        [XmlElement("resolution", typeof(string))]
        public string Resolution
        {
            get { return this.resolution; }
            set { this.resolution = value; }
        }

        [XmlElement("reportDate", typeof(DateTime))]
        public DateTime ReportDate
        {
            get { return this.reportDate; }
            set { this.reportDate = value; }
        }

        [XmlElement("lastDate", typeof(DateTime))]
        public DateTime LastDate
        {
            get { return this.lastDate; }
            set { this.lastDate = value; }
        }

    }
}
