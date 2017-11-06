using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestWCFDealServiceFees
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class TestCase
    {
      public ExpectedValues ExpectedValues { get; set; }
    }

    [Serializable]
    public class ExpectedValues
    {
        
	    private string brokerCurrency;
        private string clearingCurrency;
        private string commissionFeeCurrency;
        private string tradingFeeCurrency;
        private string entryFeeCurrency;
        private string exitFeeCurrency;


        public ExpectedValues()
        {
            
        }

        [XmlElement("BrokerFee", typeof(double))]
        public double BrokerFee { get; set; }

        [XmlElement("BrokerCurrency", typeof(string))]
        public string BrokerCurrency
        {
            get { return this.brokerCurrency; }
            set { this.brokerCurrency = value; }
        }

       
        [XmlElement("ClearingFee", typeof(double))]
        public double ClearingFee { get; set; }
       
        [XmlElement("ClearingCurrency", typeof(string))]
        public string ClearingCurrency
        {
            get { return this.clearingCurrency; }
            set { this.clearingCurrency = value; }
        }

        [XmlElement("CommissionFee", typeof(double))]
        public double CommissionFee { get; set; }
        
        [XmlElement("CommissionFeeCurrency", typeof(string))]
        public string CommissionFeeCurrency
        {
            get { return this.commissionFeeCurrency; }
            set { this.commissionFeeCurrency = value; }
        }

        [XmlElement("TradingFee", typeof(double))]
        public double TradingFee { get; set; }
       
        [XmlElement("TradingFeeCurrency", typeof(string))]
        public string TradingFeeCurrency
        {
            get { return this.tradingFeeCurrency; }
            set { this.tradingFeeCurrency = value; }
        }

        [XmlElement("EntryFee", typeof(double))]
        public double EntryFee { get; set; }
        

        [XmlElement("EntryFeeCurrency", typeof(string))]
        public string EntryFeeCurrency
        {
            get { return this.entryFeeCurrency; }
            set { this.entryFeeCurrency = value; }
        }

        [XmlElement("ExitFee", typeof(double))]
        public double ExitFee { get; set; }
        

        [XmlElement("ExitFeeCurrency", typeof(string))]
        public string ExitFeeCurrency
        {
            get { return this.exitFeeCurrency; }
            set { this.exitFeeCurrency = value; }
        }

        //[XmlElement("Fees")]
        public Fee[] Fees;

    }

    [Serializable]
    public class Fee
    {
        public Fee()
        {
            
        }

        [XmlAttribute("FeeType")]
        public string FeeType { get; set; }

        [XmlAttribute("FeeValueType")]
        public string FeeValueType { get; set; }

        [XmlAttribute("FeeValue")]
        public double FeeValue { get; set; }

        [XmlAttribute("FeeUnit")]
        public string FeeUnit { get; set; }
    }

   
}
