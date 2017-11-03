using System;
using System.Xml.Serialization;
using ElvizTestUtils.HistoricMarketDataServiceReference;


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
        private string baseCurrencyISOCode;
        private string crossCurrencyCode;
        private string currencySource;
        private DateTime lastDate;
        private DateTime firstDate;
        private string resolution;
        private GapHandling gapHandling;

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

        [XmlElement("lastDate", typeof(DateTime))]
        public DateTime LastDate
        {
            get { return this.lastDate; }
            set { this.lastDate = value; }
        }
        [XmlElement("firstDate", typeof(DateTime))]
        public DateTime FirstDate
        {
            get { return this.firstDate; }
            set { this.firstDate = value; }
        }

        [XmlElement("gapHandling", typeof(GapHandling))]
        public GapHandling GapHandling
        {
            get { return this.gapHandling; }
            set { this.gapHandling = value; }
        }

    }
}
