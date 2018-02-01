using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestWCFDealInsertWithResultMessage
{
    public class TestWCFDealInsertReportingDB
    {
        [OneTimeSetUp]
        public void Setup()
        {
            if (ConfigurationTool.PamEnabled)
            {
                ConfigurationTool.PamEnabled = false;
            }
            JobAPI.ExecuteAndAssertJob(12, 300);
            initializationConnectionToReportingDB();

            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        private const string TestFilesPath = "Testfiles\\";
        private static readonly IEnumerable<string> TestFiles = TestCasesFileEnumeratorByFolder.TestCaseFilesFiltred(TestFilesPath);
        private static string newBalanceAreaGasStorage = "NCG";

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFiles")]
        public void TestWcfDealInsertReportDB(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesPath + testFile);
           
            TestWcfDealInsertReportDBByTestFile(testFilePath);
        }


       // [Test]
        public void initializationConnectionToReportingDB()
        {
            int trId = 1;
            QAReportingDBLookUp.GetTransactionDTOFromReportingDB(trId);

        }


        private void TestWcfDealInsertReportDBByTestFile(string filePath)
        {
            string reportingDbExternalId = "";
            string reportingDbCaplId = "";
            string reportingDbUTI = "";

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath); // input XML
            string errorMessagefromXML = "";
            XmlNodeList messageNodeXML = doc.GetElementsByTagName("Message");
            if (messageNodeXML.Count > 0)
            {
                errorMessagefromXML = messageNodeXML[0].InnerText;
            }

            XmlNodeList dealNode = doc.GetElementsByTagName("TestData");
            if (dealNode.Count != 1)
            {
                Assert.Fail("Missing TestData");
            }

            //get result value from input XML
            XmlNodeList testresultNodeXml = doc.GetElementsByTagName("TestResult");
            string testresult = testresultNodeXml.Item(0).InnerText;
            
            //updating ExternlaID
            XmlNodeList externalIdNode =
                               doc.SelectNodes("/TestCase/TestData/DealInsert/Transaction/ReferenceData/ExternalId");

            if (externalIdNode != null && externalIdNode.Item(0) != null)
            {
                string externalIdNodeValue = externalIdNode.Item(0).InnerText;
                reportingDbExternalId =  "Reporting_" + externalIdNodeValue + DateTime.Now.ToString();
                externalIdNode.Item(0).InnerText = reportingDbExternalId;

            }
            else
                Assert.Fail("Missing ExternalId node/value.");

            //updating UTI
            XmlNodeList utiNode =
                               doc.SelectNodes("/TestCase/TestData/DealInsert/Transaction/CustomProperties/PropertyGroup[@name='Compliance']/StringProperty[@name='UTI']");

            if (utiNode != null && utiNode.Item(0) != null)
            {
                XmlNode uti = utiNode.Item(0);
                string utiValue = uti.Attributes["value"].Value;
                if (utiValue.Length > 22)
                    utiValue = uti.Attributes["value"].Value.Substring(0, 22);
                reportingDbUTI = "ReportinDB_" + utiValue + "_" + DateTime.Now;
                uti.Attributes["value"].Value = reportingDbUTI;
            }

            //if deal has capacity id
            XmlNodeList capacityIdNode =
                               doc.SelectNodes("/TestCase/TestData/DealInsert/Transaction/*/SettlementData/CapacityId");
           if (capacityIdNode != null && capacityIdNode.Item(0) != null) 
            {
                string capacityIdNodeValue = capacityIdNode.Item(0).InnerText;
                reportingDbCaplId = "Reporting_" + capacityIdNodeValue;
                capacityIdNode.Item(0).InnerText = reportingDbCaplId;
            }
            //update balance area frog as storage
            XmlNodeList balanceAreaNode =
                              doc.SelectNodes("/TestCase/TestData/DealInsert/Transaction/GasStorage/InstrumentData/Physical/DeliveryArea");
            if (balanceAreaNode != null && balanceAreaNode.Item(0) != null)
            {
                balanceAreaNode.Item(0).Attributes[0].Value = newBalanceAreaGasStorage;
                // if gas storage deal has delivery location it should be changes also
                XmlNodeList deliveryLocationNode = doc.SelectNodes("/TestCase/TestData/DealInsert/Transaction/GasStorage/InstrumentData/Physical/DeliveryLocation");
                if (deliveryLocationNode != null && deliveryLocationNode.Item(0) != null)
                    deliveryLocationNode.Item(0).InnerText = "None";
            }


            string result = "";

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            try
            {
                result = dealServiceClient.ImportDeal(dealNode[0].InnerXml);
                //  Console.WriteLine(dealNode[0].InnerXml);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                if (testresult.ToUpper() == "EXCEPTION")
                {
                    Assert.AreEqual(errorMessagefromXML, ex.Message, "Other exception was caught");
                    return;
                }
                Assert.Fail(ex.Message);
            }

            //create and read XML with ImportDeal's results
            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(result);

            XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
            string dealinsertResult = resultNode[0].InnerText;

            Assert.AreEqual(1, resultNode.Count, "Result Message is not valid, it has multipple results");

            if (!(testresult.ToUpper() == "SUCCESS" && dealinsertResult.ToUpper() == "SUCCESS"))
            {
                if (testresult.ToUpper() == "FAILURE" && dealinsertResult.ToUpper() == "FAILURE")
                {
                    XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");
                    string errorMessage = messageNode[0].InnerText;
                    // Console.WriteLine(errorMessage);
                    //Assert error message
                    Assert.AreEqual(errorMessagefromXML, errorMessage, "Not equal error messages: ");

                    return;
                }

                if (testresult.ToUpper() == "FAILURE" && dealinsertResult.ToUpper() == "SUCCESS")
                    Assert.Fail("Expected Failure, but was : " + dealinsertResult + " \n Expected Error Message : " +
                                errorMessagefromXML);

                if (testresult.ToUpper() == "SUCCESS" && dealinsertResult.ToUpper() == "FAILURE")
                {
                    string actualErrorString = "";
                    if (resultXml.GetElementsByTagName("Message").Count > 0)
                        actualErrorString = resultXml.GetElementsByTagName("Message")[0].InnerText;

                    Assert.Fail("Expected Success, but was Failure: " + " \n ErrorMessage : " + actualErrorString);
                }
                Assert.Fail("Unexpected TestResult : " + dealinsertResult + " \n Expected : " + testresult +
                            "\n ErrorMessage : " + resultXml.GetElementsByTagName("Message")[0].InnerText);
            }

            XmlNodeList assertValue = doc.GetElementsByTagName("AssertQaTransactionDTO");
            Assert.AreEqual(1, assertValue.Count, "Test case doesn't have 'AssertQaTransactionDTO' XML-node.");

            if (assertValue[0].InnerText.ToUpper() == "TRUE")
            {
                XmlNodeList IdNode = resultXml.GetElementsByTagName("TransactionId");
                int insertedTransactionID = Convert.ToInt32(IdNode[0].InnerText);

                //Console.WriteLine("TR_ID= " + insertedTransactionID);
                XmlNodeList expectedDtoNode = doc.GetElementsByTagName("QaTransactionDTO");
                if (expectedDtoNode.Count > 0)
                {
                    Thread.Sleep(7000);
                    CompareTransactionDto(filePath, insertedTransactionID, reportingDbExternalId, reportingDbCaplId, reportingDbUTI);
                }
                else
                {
                    Assert.Fail("Test case doesn't have 'AssertQaTransactionDTO' XML-node.");
                }
            }
        }

        public static void CompareTransactionDto(string testFilePath, int insertedTransactionID,
            string reportingDbExternalId="", string reportingDbCaplId ="", string reportingDbUti ="")
        {

            QaTransactionDTO reportingDbDto = QAReportingDBLookUp.GetTransactionDTOFromReportingDB(insertedTransactionID);

            XmlSerializer serializer = new XmlSerializer(typeof (QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(testFilePath);
            reader.ReadToDescendant("QaTransactionDTO");

            QaTransactionDTO expectedTransactionDto = (QaTransactionDTO) serializer.Deserialize(reader.ReadSubtree());
            reader.Close();
            //update some properties
            if (reportingDbExternalId != "")
                expectedTransactionDto.ReferenceData.ExternalId = reportingDbExternalId;

            if (reportingDbCaplId != "") expectedTransactionDto.InstrumentData.CapacityId = reportingDbCaplId;

            if (reportingDbUti != "")
            {
                IList<PropertyGroup> propGroup = expectedTransactionDto.PropertyGroups.ToList();
                PropertyGroup propertyCompliance = propGroup.FirstOrDefault(x => x.Name == "Compliance");
                if (propertyCompliance != null)
                {
                    List<Property> propList = propertyCompliance.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "UTI");
                    if (item != null) item.Value = reportingDbUti;
                }
            }

            if (expectedTransactionDto.DealType == "Currency-European" || expectedTransactionDto.DealType == "Emission-European")
                expectedTransactionDto.InstrumentData.UnderlyingInstrumentType = "Forward";

            if (expectedTransactionDto.DealType == "Elcertificate-StructuredDeal")
                expectedTransactionDto.ContractModelType = "Fixed Volume";

            if (expectedTransactionDto.DealType == "Gas-Storage")
            {
                expectedTransactionDto.InstrumentData.BalanceArea = newBalanceAreaGasStorage;
                expectedTransactionDto.InstrumentData.PriceType = "Fixed";
                //works only for one test case PassGasStorageLocation.xml
                if (expectedTransactionDto.DeliveryLocation != null) expectedTransactionDto.DeliveryLocation = null;
            }
            //can be removed when fixed ELVIZ-11352
            if (expectedTransactionDto.DealType == "Gas-IndexedOption" || expectedTransactionDto.DealType == "Electricity-FTROption")
            {
                reportingDbDto.InstrumentData.ExpiryDate = null;
            }
           
            if (expectedTransactionDto.DealType == "Electricity-ReserveCapacity")
            {
                reportingDbDto.ContractModelType = "Flexible Volume";
                expectedTransactionDto.InstrumentData.PriceType = "Fixed";
            }
            if (expectedTransactionDto.DealType.Contains("Green Certificate")) expectedTransactionDto.DealType = expectedTransactionDto.DealType.Replace(" ", "");

            if (expectedTransactionDto.DealType == "Electricity-FixedPriceFloatingVolume")
                expectedTransactionDto.SettlementData.QuantityUnit = "%";

              string priceVolumeUnit = expectedTransactionDto.SettlementData.PriceVolumeUnit;

            switch (priceVolumeUnit)
            {
                case "PerMWh":
                    expectedTransactionDto.SettlementData.PriceVolumeUnit = "MWh";
                break;
            }
            //string tr_status = expectedTransactionDto.DealDetails.Status;
            //switch (tr_status)
            //{
            //    case "Onhold":
            //        expectedTransactionDto.DealDetails.Status = "Onhold";
            //        break;
            //}
            //  string[] excludeProps = new string[] { "TransactionId", "ReferenceData.ModificationDateTimeUtc", "ReferenceData.ReferringId", "TransactionWorkFlowDetails.TimeStampAuthorised", "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised" };

            string[] excludeProps = new string[]
           {
                "TransactionId", "ReferenceData.ModificationDateTimeUtc", "ReferenceData.ReferringId", "TransactionWorkFlowDetails.TimeStampAuthorised", "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised" ,
                "SettlementData.PriceVolumeUnit",
           //     "SettlementData.MarketPriceMultiplicator",
                "SettlementData.Resolution",
                "SettlementData.PriceVolumeTimeSeriesDetails",
                "InstrumentData.InstrumentName",
               // "InstrumentData.Strike",
               // "InstrumentData.PriceType",
               // "InstrumentData.CapFloorPricingResolution",
                "ReferenceData.Deliveries",
                "ReferenceData.DistributedQuantity",//*
                "ReferenceData.OriginalQuantity",
                "TransactionWorkFlowDetails.Authorised",
               // "Broker",
                "SettlementData.PriceVolumeTimeSeriesDetails",
                "SettlementData.TimeSeriesSet",
                "SettlementData.QuantityUnit",//needs to fix
             //   "InstrumentData.PriceBasisToArea"//needs to fix


    };

        QaTransactionDtoAssert.AreEqual(expectedTransactionDto, reportingDbDto, excludeProps, false);
        }

    }
}
