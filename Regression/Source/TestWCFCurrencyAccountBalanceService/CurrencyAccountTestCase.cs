using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ElvizTestUtils;
using NUnit.Framework;

namespace TestWCFCurrencyAccountBalanceService
{
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    public class CurrencyAccountTestCase
    {
        public InputData InputData { get; set; }
        public ExpectedResult[] ExpectedResults { get; set; }
    }


    public class InputData
    {
        public Insert[] CurrencyAccountInserts { get; set; }

        

    }

    public class GetMethod
    {
        public bool MostRecent { get; set; }
        public DateRangeCriteria DateRangeCriteria { get; set; }
    }

    public class DateRangeCriteria
    {
        public DateTime BalanceDateFrom { get; set; }
        public DateTime BalanceDateTo { get; set; }
    }

    public class ExpectedResult
    {
        [XmlAttribute]
        public string Description { get; set; }
        public GetMethod GetMethod { get; set; }
        public CurrencyAccount[] CurrencyAccounts { get; set; }
    }


    public class Insert
    {
        [XmlAttribute]
        public DateTime InsertBalanceDate { get; set; }
 
       public CurrencyAccount[] CurrencyAccounts { get; set; }
    }

 
    public class CurrencyAccount
    {
        [XmlAttribute]
        public string CurrencyIsoCode { get; set; }
        
        [XmlAttribute]
        public DateTime BalanceDate { get; set; }

        //[XmlAttribute(AttributeName = "BalanceDate")]
        //public string BalanceDateString
        //{
        //    get
        //    {
        //        if (BalanceDate==null)
        //        {
        //            return string.Empty;
        //        }
        //        return TestXmlTool.ConvertToXmlDateString(BalanceDate);
        //    }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //        {
        //            BalanceDate =null;           
        //        }
        //        else{
        //            BalanceDate = TestXmlTool.ConvertXmlDate(value);
        //        }
        //    }}

        
        [XmlAttribute]
        public double NominalAmount { get; set; }

        [XmlAttribute]
        public string UpdatedBy { get; set; }

        //[XmlAttribute]
        //public DateTime UpdatedTimeStampUtc { get; set; }

        //[XmlAttribute(AttributeName = "UpdatedTimeStampUtc")]
        //public string UpdatedTimeStampUtcString
        //{
        //    get
        //    {
        //        if (UpdatedTimeStampUtc == null)
        //        {
        //            return string.Empty;
        //        }
        //        return TestXmlTool.ConvertToXmlDateString(UpdatedTimeStampUtc);
        //    }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //        {
        //            UpdatedTimeStampUtc = null;
        //        }
        //        else
        //        {
        //            UpdatedTimeStampUtc = TestXmlTool.ConvertXmlDate(value);
        //        }
                
        //    }

        //}
    }
}
