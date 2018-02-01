using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;


namespace TestWCFDealServiceFees
{
        [TestFixture]
        public class InserDealAndCheckFees
        {

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        private TestCase DeserializeXml(string testFilepath)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (TestCase));

                string filepath = Path.Combine(Directory.GetCurrentDirectory(),
                                               "TestFiles\\" + testFilepath);

                FileStream readFileStream = File.Open(
                    filepath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                TestCase testCase= (TestCase) xmlSerializer.Deserialize(readFileStream);
                
                readFileStream.Close();

                readFileStream.Dispose();

                return testCase;
            }
            private static readonly IEnumerable<string> TestFilesFees = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesFees")]
            public void CompareFees(string testFilepath)
            {

                TestCase test = this.DeserializeXml(testFilepath);

                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();

                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFilepath);

                XmlDocument inputDoc = new XmlDocument();
                inputDoc.Load(filepath);
                //get "DealInsert" part from input XML
                XmlNodeList elemList = inputDoc.GetElementsByTagName("DealInsert");
                // Create Deal with data from XML              
                string resultImportDeal = service.ImportDeal(elemList[0].OuterXml);

               // Console.WriteLine(resultImportDeal);

                // Create the tmp XmlDocument with ImportDeal resuls 
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resultImportDeal);

                int trID = 0; //global variable
                string errorMessage = "Error. ImportDeal() result != success.";

                XmlNodeList nodeRes = doc.GetElementsByTagName("Result");
                XmlNodeList nodeMes = doc.GetElementsByTagName("Message");
                if (nodeMes.Count > 0)
                {
                    errorMessage = nodeMes[0].InnerText;
                }

                Assert.AreEqual("Success", nodeRes[0].InnerText, errorMessage);

                XmlNodeList nodeId = doc.GetElementsByTagName("TransactionId");
                if (nodeId.Count > 0)
                {
                    trID = Convert.ToInt32(nodeId[0].InnerText);
                    //Console.WriteLine("Transaction id = " + trID);
                }
                else Console.WriteLine("Error when parsing ImportDeal result");
                //Get transaction by id using GetTransactionsByIds()
                if (trID != 0)
                {
                    ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                    int[] ids = {trID};
                    TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);
                    //if (transactions.Length > 0)
                    //{

                        //Compare Expected values from XML (deserializing class) and transactionDTO properties
                        Assert.AreEqual(test.ExpectedValues.BrokerFee, transactions[0].BrokerFee,
                                        "Wrong result for BrokerFee:");
                        Assert.AreEqual(test.ExpectedValues.BrokerCurrency, transactions[0].BrokerFeeCurrency.ISOCode,
                                        "Wrong result for BrokerFeeCurrency:");

                        Assert.AreEqual(test.ExpectedValues.ClearingFee, transactions[0].ClearingFee,
                                        "Wrong result for ClearingFee");
                        Assert.AreEqual(test.ExpectedValues.ClearingCurrency, transactions[0].ClearingCurrency.ISOCode,
                                        "Wrong result for ClearingCurrency ");

                        Assert.AreEqual(test.ExpectedValues.CommissionFee, transactions[0].CommissionFee,
                                        "Wrong result for CommissionFee ");
                        Assert.AreEqual(test.ExpectedValues.CommissionFeeCurrency,
                                        transactions[0].CommissionFeeCurrency.ISOCode,
                                        "Wrong result for CommissionFeeCurrency:");

                        Assert.AreEqual(test.ExpectedValues.TradingFee, transactions[0].TradingFee,
                                        "Wrong result for TradingFee:");
                        Assert.AreEqual(test.ExpectedValues.TradingFeeCurrency,
                                        transactions[0].TradingFeeCurrency.ISOCode,
                                        "Wrong result for TradingFeeCurrency:");

                        Assert.AreEqual(test.ExpectedValues.EntryFee, transactions[0].EntryFee,
                                        "Wrong result for EntryFee:");


                        if (transactions[0].EntryFeeCurrency != null)
                        {
                            Assert.AreEqual(test.ExpectedValues.EntryFeeCurrency, transactions[0].EntryFeeCurrency.ISOCode,
                                "Wrong result for EntryFeeCurrency:");
                        }
                        Assert.AreEqual(test.ExpectedValues.ExitFee, transactions[0].ExitFee,
                                            "Wrong result for ExitFee:");
                        if (transactions[0].ExitFeeCurrency != null)
                        {
                            Assert.AreEqual(test.ExpectedValues.ExitFeeCurrency, transactions[0].ExitFeeCurrency.ISOCode,
                                "Wrong result for ExitFeeCurrency:");
                        }


                    ElvizTestUtils.LookUpServiceReference.Fee[] dtoFees = transactions[0].Fees;
                    if (test.ExpectedValues.Fees != null && dtoFees !=null )
                    {
                       

                        List<Fee> actualFees = new List<Fee>();

                        foreach (ElvizTestUtils.LookUpServiceReference.Fee dtoFee in dtoFees)
                        {
                            Fee fee = new Fee()
                            {
                                FeeType = dtoFee.FeeType,
                                FeeUnit = dtoFee.FeeUnit,
                                FeeValue = dtoFee.FeeValue,
                                FeeValueType = dtoFee.FeeValueType
                            };
                            actualFees.Add(fee);
                        }

                        actualFees =
                            actualFees.Where(x => x.FeeValue != 0.0).OrderBy(x => x.FeeType + x.FeeValueType).ToList();
                        List<Fee> expectedFees = test.ExpectedValues.Fees.OrderBy(x => x.FeeType + x.FeeValueType).ToList();

                        for (int i = 0; i < expectedFees.Count; i++)
                        {
                            Fee expectedFee = expectedFees[i];
                            Fee actualFee = actualFees[i];

                            Assert.AreEqual(expectedFee.FeeType, actualFee.FeeType, "Wrong Fee Type");
                            Assert.AreEqual(expectedFee.FeeValueType, actualFee.FeeValueType, "Wrong Fee Value Type");
                            Assert.AreEqual(expectedFee.FeeValue, actualFee.FeeValue, "Wrong Fee Value");
                            Assert.AreEqual(expectedFee.FeeUnit, actualFee.FeeUnit, "Wrong Fee Unit");

                        }

                    }
                    // }
                }
            }
        }

 
}
