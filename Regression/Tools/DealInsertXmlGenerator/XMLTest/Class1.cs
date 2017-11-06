//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Xml.Serialization;
//using NUnit.Framework;
//using ExcelTransactionWrapper;

//namespace XMLTest
//{
//    class MyClass
//    {
//        private string myString;

//        public string MyString
//        {
//            get { return myString; }
//            set { myString = value; }
//        }
//    }




//    [TestFixture]
//    public class Class1
//    {
//        [Test]
//        public void TestT()
//        {
//               TransactionWrapper d=new TransactionWrapper();
//            const string value = "TT";
//            Helper.SetProp(d, "LoadProfile", value);

//            Assert.AreEqual(value,d.LoadProfile);

//        }

//        [Test]
//        public void Test()
//        {

//            DateTime fromDate=new DateTime(2012,1,1);
//            DateTime toDate=new DateTime(2012,1,31);
//            string loadProfile = "Base";
//            string priceBasis = "EEX";
//            DateTime tradeDate=new DateTime(2011,1,1);
//            string buySell = "buy";
//            string currency = "eur";
//            string path_to_xml = @"C:\elviz\data\apifiles\tt.xml";
//            string executionVenue = "eex";
//            string currencySource = "ecb";
//            string portfolio = "API Test Customer";
//            string counterpartyPortfolio = "API Test Counterparty";
//            string financialPhysical = "physical";
//            double price = 40;
//            double quantity = 100;
//            string externalId = "T-XML-1";


//            DealInsertXmlGenerator generator=new DealInsertXmlGenerator();
//            generator.AddElectricityForward(portfolio,
//                                            counterpartyPortfolio,
//                                            buySell, executionVenue,
//                                            priceBasis, loadProfile,
//                                            fromDate, toDate,
//                                            financialPhysical, price,
//                                            quantity, currency,
//                                            currencySource, tradeDate,
//                                            "a");

//            generator.AddElectricityForward(portfolio,
//                                counterpartyPortfolio,
//                                buySell, executionVenue,
//                                priceBasis, loadProfile,
//                                fromDate, toDate,
//                                financialPhysical, price,
//                                quantity, currency,
//                                currencySource, tradeDate,
//                                "b");


//            generator.GenerateBulkInsertXmlFile(path_to_xml);

//        }
//    }
//}
