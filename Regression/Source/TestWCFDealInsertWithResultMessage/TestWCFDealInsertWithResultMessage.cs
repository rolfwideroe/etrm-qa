using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestWCFDealInsertWithResultMessage
{
    [TestFixture]
    public class TestWcfDealInsertWithResultMessage
    {
        
        
        [TestFixtureSetUp]
        public void Setup()
        {
            if (ConfigurationTool.PamEnabled)
            {
                ConfigurationTool.PamEnabled = false;
            }

            if (ConfigurationTool.AutorizationEnabled)
            {
                ConfigurationTool.AutorizationEnabled = true;
            }

        }

        private const string TestFilesWithResultMessagePath = "Testfiles\\";
        private const string TestFilesWithResultMessageCustomPropertiesPath = "TestFilesCustomProperties\\";
        private const string TestFilesWithResultMessageFeesPath = "TestFilesFees\\";
        private const string TestFilesWithResultMessageCurrencySwapPath = "TestFilesCurrencySwap\\";

        private static readonly IEnumerable<string> TestFilesWithResultMessage = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilesWithResultMessagePath);
        private static readonly IEnumerable<string> TestFilesWithResultMessageCustomProperties = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilesWithResultMessageCustomPropertiesPath);
        private static readonly IEnumerable<string> TestFilesWithResultMessageFees = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilesWithResultMessageFeesPath);
        private static readonly IEnumerable<string> TestFilesWithResultMessageCurrencySwap = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilesWithResultMessageCurrencySwapPath);

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesWithResultMessage")]
        public void TestWcfDealInsertTestFromXmlFile(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesWithResultMessagePath + testFile);

            TestWcfDealInsertByTestFile(testFilePath);
        }

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesWithResultMessageCustomProperties")]
        public void TestWcfDealInsertCustomProperties(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesWithResultMessageCustomPropertiesPath + testFile);

            TestWcfDealInsertByTestFile(testFilePath);
        }

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesWithResultMessageFees")]
        public void TestWcfDealInsertFees(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesWithResultMessageFeesPath + testFile);

            TestWcfDealInsertByTestFile(testFilePath);
        }

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesWithResultMessageCurrencySwap")]
        public void TestWcfWithResultMessageCurrencySwap(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesWithResultMessageCurrencySwapPath + testFile);

            TestWcfDealInsertByTestFileCurrencySwap(testFilePath);
        }


        public static void TestWcfDealInsertByTestFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            
            doc.Load(filePath); // input XML
            string errorMessagefromXML = "";
            XmlNodeList messageNodeXML = doc.GetElementsByTagName("Message");
            if (messageNodeXML.Count > 0)
            {
                errorMessagefromXML = messageNodeXML[0].InnerText;
                errorMessagefromXML = errorMessagefromXML.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");
            }

            XmlNodeList dealNode = doc.GetElementsByTagName("TestData");
            if (dealNode.Count != 1)
            {
                Assert.Fail("Missing TestData");
            }

            //get result value from input XML
            XmlNodeList testresultNodeXml = doc.GetElementsByTagName("TestResult");
            string testresult = testresultNodeXml.Item(0).InnerText;

            string result = "";

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            try
            {
                result = dealServiceClient.ImportDeal(dealNode[0].InnerXml);
                //Console.WriteLine(dealNode[0].InnerXml);
              //  Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);

                if (testresult.ToUpper() == "EXCEPTION")
                {
                    string exMessage = ex.Message.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");
                    Assert.AreEqual(errorMessagefromXML, exMessage, "Other exception was caught");
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
                    //Console.WriteLine(errorMessagefromXML);
                    //Assert error message

                    //removed \r,\n - failed for Azure machines
                    errorMessage = errorMessage.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");

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
                // Console.WriteLine("TR_ID= " + insertedTransactionID);
                XmlNodeList expectedDtoNode = doc.GetElementsByTagName("QaTransactionDTO");
                if (expectedDtoNode.Count > 0)
                {
                    CompareTransactionDto(filePath, insertedTransactionID);
                }
                else
                {
                    Assert.Fail("Test case doesn't have 'AssertQaTransactionDTO' XML-node.");
                }
            }
        }

        private void TestWcfDealInsertByTestFileCurrencySwap(string filePath)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(filePath); // input XML
            string errorMessagefromXML = "";
            XmlNodeList messageNodeXML = doc.GetElementsByTagName("Message");
            if (messageNodeXML.Count > 0)
            {
                errorMessagefromXML = messageNodeXML[0].InnerText;
                errorMessagefromXML = errorMessagefromXML.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");
            }

            XmlNodeList dealNode = doc.GetElementsByTagName("TestData");
            if (dealNode.Count != 1)
            {
                Assert.Fail("Missing TestData");
            }

            XmlNodeList assertValue = doc.GetElementsByTagName("AssertQaBulkTransactionDTO");
            Assert.AreEqual(1, assertValue.Count, "Test case doesn't have 'AssertQaBulkTransactionDTO' XML-node.");

            //get result value from input XML
            XmlNodeList testresultNodeXml = doc.GetElementsByTagName("TestResult");
            string testresult = testresultNodeXml.Item(0).InnerText;

            string result = "";

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            try
            {
                result = dealServiceClient.ImportDeal(dealNode[0].InnerXml);
                //Console.WriteLine(dealNode[0].InnerXml);
                //   Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);

                if (testresult.ToUpper() == "EXCEPTION")
                {
                    string exMessage = ex.Message.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");
                    Assert.AreEqual(errorMessagefromXML, exMessage, "Other exception was caught");
                    return;
                }
                Assert.Fail(ex.Message);
            }

            //create and read XML with ImportDeal's results
            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(result);

            XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
            string dealinsertResult = resultNode[0].InnerText;

            Assert.AreEqual(1, resultNode.Count, "Result Message is not valid, it has multiple results");

            if (!(testresult.ToUpper() == "SUCCESS" && dealinsertResult.ToUpper() == "SUCCESS"))
            {
                if (testresult.ToUpper() == "FAILURE" && dealinsertResult.ToUpper() == "FAILURE")
                {
                    XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");
                    string errorMessage = messageNode[0].InnerText;
                    // Console.WriteLine(errorMessage);
                    
                    //removed \r,\n - failed for Azure machines
                    errorMessage = errorMessage.Replace("\r", "").Replace("\n", "").Replace("(1)", "").Replace("\t", "");

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

            //Compare DealInsert Results with QaDto
            if (assertValue[0].InnerText.ToUpper() == "TRUE")
            {

                XmlNodeList IdNode = resultXml.GetElementsByTagName("ExternalTransactionId");
                string[] externalIds = IdNode[0].InnerText.Split(';');
                
                XmlNodeList expectedDtoNodes = doc.GetElementsByTagName("QaTransactionDTO");

                if (externalIds.Length != expectedDtoNodes.Count)
                {
                    Assert.Fail("Amount of externalIds doesn't match amount of test QaBulkTransactionDTO nodes.");
                }

                for (int i = 0; i < externalIds.Length; i++)
                {
                    externalIds[i] = externalIds[i].Trim();
                    Console.WriteLine("ExternalId=" + externalIds[i]);

                    CompareCurrencySwapTransactionDtos(filePath, externalIds);
                }
            }
        }

        private void CompareCurrencySwapTransactionDtos(string testFilePath, string[] externalIds)
        {
            QaLookUpClient c = new QaLookUpClient();

            List<QaTransactionDTO> resultDtos = c.GetQaTransactionDtos(externalIds);

            XmlSerializer serializer = new XmlSerializer(typeof (QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(testFilePath);

            List<QaTransactionDTO> expectedTransactionDtos = new List<QaTransactionDTO>();

            while (reader.ReadToDescendant("QaTransactionDTO"))
            {
                QaTransactionDTO expectedTransactionDto = (QaTransactionDTO) serializer.Deserialize(reader.ReadSubtree());

                expectedTransactionDtos.Add(expectedTransactionDto);
            }
            reader.Close();

            string[] excludeProps = { "TransactionId", "ReferenceData.ModificationDateTimeUtc", "ReferenceData.ReferringId", "ReferenceData.ContractSplitId", "TransactionWorkFlowDetails.TimeStampAuthorised", "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised" };

            for (int i = 0; i < expectedTransactionDtos.Count; i++)
            {
                QaTransactionDtoAssert.AreEqual(expectedTransactionDtos[i], resultDtos[i], excludeProps, false);
            }
        }

        public static void CompareTransactionDto(string testFilePath, int insertedTransactionID)
        {
         ///   string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            //get inserted transactionDTO
            QaLookUpClient c = new QaLookUpClient();

            QaTransactionDTO dto = c.GetQaTransactionDTO(insertedTransactionID);

            //if ((dto.InstrumentData.ReferencePriceSeries != null) && (!dto.DealType.Contains("Emission"))
            //    && (!dto.DealType.Contains("Elcertificate")) && (!dto.DealType.Contains("GreenCertificate"))
            //    ) AddNewNode2QaTransactionDTOXml(testFilePath, dto.InstrumentData.ReferencePriceSeries);


            XmlSerializer serializer = new XmlSerializer(typeof (QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(testFilePath);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO expectedTransactionDTO = (QaTransactionDTO) serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            string[] excludeProps = new string[] { "TransactionId", "ReferenceData.ModificationDateTimeUtc", "ReferenceData.ReferringId", "TransactionWorkFlowDetails.TimeStampAuthorised", "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised" };

        
            QaTransactionDtoAssert.AreEqual(expectedTransactionDTO,dto,excludeProps,false);
        }

     //   [Test]
        public static void AddNewNode2QaTransactionDTOXml(string testFile, string value)
        {
           // string testFile = @"C:\TFS\Development\QA\Regression\Bin\TestWCFDealInsertWithResultMessage\TestFiles\PassElectricity-FixedPriceFloatingVolume.xml";

            XmlDocument doc = new XmlDocument();

            doc.Load(testFile); // input XML

            XmlNode newElement = doc.CreateElement("ReferencePriceSeries");
            newElement.InnerText = value ;

            XmlNodeList instData = doc.SelectNodes("/TestCase/QaTransactionDTO/InstrumentData");
            if (instData.Count > 0)
            {
                XmlNode timezoneNode = instData.Item(0).SelectSingleNode("TimeZone");
                XmlNode refPS = instData.Item(0).SelectSingleNode("ReferencePriceSeries");
                if (refPS == null)
                    instData.Item(0).InsertAfter(newElement, timezoneNode);
                else
                {
                    refPS.InnerText = value;
                }
               // else Assert.Fail("remove second node");
            }

            doc.Save(testFile);

            Console.WriteLine("Added new element" + instData.Item(0).OuterXml);
           
        }

        public void CompareTransactionDtoOld(string testFile, int insertedTransactionID)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            //get inserted transactionDTO
            QaLookUpClient c = new QaLookUpClient();

            QaTransactionDTO dto = c.GetQaTransactionDTO(insertedTransactionID);
         
            XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(testFilePath);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO expectedTransactionDTO = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            //Assert.AreEqual(expectedTransactionDTO.TransactionId, insertedTransactionID);

            Assert.AreEqual(expectedTransactionDTO.DealType, dto.DealType, "Values for property 'DealType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.BuySell, dto.BuySell, "Values for property 'BuySell' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DeliveryType, dto.DeliveryType, "Values for property 'DeliveryType' are not equal:");
            ////Portfolio
            Assert.AreEqual(expectedTransactionDTO.Portfolios.PortfolioName, dto.Portfolios.PortfolioName, "Values for property 'PortfolioName' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.PortfolioExternalId, dto.Portfolios.PortfolioExternalId, "Values for property 'PortfolioExternalId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.CounterpartyPortfolioExternalId, dto.Portfolios.CounterpartyPortfolioExternalId, "Values for property 'CounterpartyPortfolioExternalId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.CounterpartyPortfolioName, dto.Portfolios.CounterpartyPortfolioName, "Values for property 'CounterpartyPortfolioName'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ContractModelType,dto.ContractModelType,"Values for property 'ContractModelType' are not equal");
            //Console.WriteLine(expectedTransactionDTO.TransactionId);
            //Console.WriteLine(expectedTransactionDTO.DealType);
            //Console.WriteLine(expectedTransactionDTO.Portfolios.PortfolioName);

            ////InstrumentData
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExecutionVenue, dto.InstrumentData.ExecutionVenue, "Values for property 'ExecutionVenue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.InstrumentName, dto.InstrumentData.InstrumentName, "Values for property 'InstrumentName' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceBasis, dto.InstrumentData.PriceBasis, "Values for property 'PriceBasis'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceBasisToArea, dto.InstrumentData.PriceBasisToArea, "Values for property 'PriceBasisToArea'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceType, dto.InstrumentData.PriceType, "Values for property 'PriceType'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CapFloorPricingResolution, dto.InstrumentData.CapFloorPricingResolution, "Values for property 'CapFloorPricingResolution' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.FromDate, dto.InstrumentData.FromDate, "Values for property 'FromDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ToDate, dto.InstrumentData.ToDate, "Values for property 'ToDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.LoadProfile, dto.InstrumentData.LoadProfile, "Values for property 'LoadProfile' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.TimeZone, dto.InstrumentData.TimeZone, "Values for property 'TimeZone' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PutCall, dto.InstrumentData.PutCall, "Values for property 'PutCall' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.BalanceArea, dto.InstrumentData.BalanceArea, "Values for property 'BalanceArea' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Strike, dto.InstrumentData.Strike, "Values for property 'Strike' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryTime, dto.InstrumentData.ExpiryTime, "Values for property 'ExpiryTime' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryDate, dto.InstrumentData.ExpiryDate, "Values for property 'ExpiryDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.IndexFormula, dto.InstrumentData.IndexFormula, "Values for property 'IndexFormula' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.BaseIsoCurrency, dto.InstrumentData.BaseIsoCurrency, "Values for property 'BaseIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CrossIsoCurrency, dto.InstrumentData.CrossIsoCurrency, "Values for property 'CrossIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.TransferDate, dto.InstrumentData.TransferDate, "Values for property 'TransferDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CertificateType, dto.InstrumentData.CertificateType, "Values for property 'CertificateType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ProductionFacility, dto.InstrumentData.ProductionFacility, "Values for property 'ProductionFacility' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.EnvironmentLabel, dto.InstrumentData.EnvironmentLabel, "Values for property 'EnvironmentLabel' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.FromCountry, dto.InstrumentData.FromCountry, "Values for property 'FromCountry' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ToCountry, dto.InstrumentData.ToCountry, "Values for property 'ToCountry' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingPeriod, dto.InstrumentData.SamplingPeriod, "Values for property 'SamplingPeriod' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Interconnector, dto.InstrumentData.Interconnector, "Values for property 'Interconnector' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.LossFactor, dto.InstrumentData.LossFactor, "Values for property 'LossFactor' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryOffset, dto.InstrumentData.ExpiryOffset, "Values for property 'ExpiryOffset' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Priority, dto.InstrumentData.Priority, "Values for property 'Priority' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.MinVol, dto.InstrumentData.MinVol, "Values for property 'MinVol' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.MaxVol, dto.InstrumentData.MaxVol, "Values for property 'MaxVol' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CapacityId, dto.InstrumentData.CapacityId, "Values for property 'CapacityId' are not equal:");

            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingExecutionVenue, dto.InstrumentData.UnderlyingExecutionVenue, "Values for property 'UnderlyingExecutionVenue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingDeliveryType, dto.InstrumentData.UnderlyingDeliveryType, "Values for property 'UnderlyingDeliveryType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingInstrumentType, dto.InstrumentData.UnderlyingInstrumentType, "Values for property 'UnderlyingInstrumentType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingFrom, dto.InstrumentData.SamplingFrom, "Values for property 'SamplingFrom' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingTo, dto.InstrumentData.SamplingTo, "Values for property 'SamplingTo' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.VolumeReferenceExternalId, dto.InstrumentData.VolumeReferenceExternalId, "Values for property 'VolumeReferenceExternalId' are not equal:");

            ////SettlementData
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Quantity, dto.SettlementData.Quantity, "Values for property 'Quantity' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.QuantityUnit, dto.SettlementData.QuantityUnit, "Values for property 'QuantityUnit are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Price, dto.SettlementData.Price, "Values for property 'Price' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceIsoCurrency, dto.SettlementData.PriceIsoCurrency, "Values for property 'PriceIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeUnit, dto.SettlementData.PriceVolumeUnit, "Values for property 'PriceVolumeUnit' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.CurrencySource, dto.SettlementData.CurrencySource, "Values for property 'CurrencySource' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.MarketPriceMultiplicator, dto.SettlementData.MarketPriceMultiplicator, "Values for property 'MarketPriceMultiplicator' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.MasterAgreement, dto.SettlementData.MasterAgreement, "Values for property 'MasterAgreement' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.SettlementRule, dto.SettlementData.SettlementRule, "Values for property 'SettlementRule' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Resolution, dto.SettlementData.Resolution, "Values for property 'Resolution' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.NominationsHourly, dto.SettlementData.NominationsHourly, "Values for property 'NominationsHourly' are not equal:");
           
            if (expectedTransactionDTO.SettlementData.TimeSeriesSet != null)
            {
                for (int i = 0; i < expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution,
                        "Values for property 'TimeSeriesSet.TimeSeries.Resolution' are not equal at index = " + i);

                    Assert.AreEqual(expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName,
                        "Values for property 'TimeSeriesSet.TimeSeries.TimeSeriesTypeName' are not equal at index = " + i);

                    Assert.AreEqual(expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName,
                        "Values for property 'TimeSeriesSet.TimeSeries.TimezoneName' are not equal at index = " + i);

                    TimeSeriesValue[] expectedTSV = expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;
                    TimeSeriesValue[] actualTSV = dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;

                    for (int j = 0; j < expectedTSV.Length; j++)
                    {
                        Assert.AreEqual(expectedTSV[j].FromDateTime, actualTSV[j].FromDateTime, "Values for property 'FromDateTime' for '" + expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j);
                        Assert.AreEqual(expectedTSV[j].ToDateTime, actualTSV[j].ToDateTime, "Values for property 'ToDateTime' for '" + expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j);
                        Assert.AreEqual(expectedTSV[j].Value, actualTSV[j].Value, "Values for property 'FeeValue' for '" + expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j);
                    }
                }
            }

            // add PriceVolumeTimeSeries
            if (expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails != null)
            {
                Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails.Count(), dto.SettlementData.PriceVolumeTimeSeriesDetails.Count(), "Values for property 'PriceVolumeTimeSeriesDetails' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime, dto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime, "'PriceVolumeTimeSeriesDetails[" +i +"].FromDate' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime, dto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime, "'PriceVolumeTimeSeriesDetails[" + i + "].ToDate' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].Price, dto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price, "'PriceVolumeTimeSeriesDetails[" + i + "].Price' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume, dto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume, "'PriceVolumeTimeSeriesDetails[" + i + "].Volume' properties are not equal:");
                }
            }

            ////DealDetails
            Assert.AreEqual(expectedTransactionDTO.DealDetails.Trader, dto.DealDetails.Trader, "Values for property 'Trader' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DealDetails.Status, dto.DealDetails.Status, "Values for property 'Status' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DealDetails.TradeDateTimeUtc, dto.DealDetails.TradeDateTimeUtc, "Values for property 'TradeDateTimeUtc' are not equal:");
            ////FeesData
            Assert.AreEqual(expectedTransactionDTO.FeesData.Broker, dto.FeesData.Broker, "Values for property 'Broker' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.FeesData.ClearingType, dto.FeesData.ClearingType, "Values for property 'ClearingType' are not equal:");


            if (expectedTransactionDTO.FeesData.Fees != null)
            {
                Assert.AreEqual(expectedTransactionDTO.FeesData.Fees.Count(), dto.FeesData.Fees.Count(), "Values for property 'Fees' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.FeesData.Fees.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeType, dto.FeesData.Fees[i].FeeType, "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".ValueType' properties at index= [" + i + "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeValue, dto.FeesData.Fees[i].FeeValue, "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".FeeValue' properties at index= [" + i + "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeUnit, dto.FeesData.Fees[i].FeeUnit, "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".FeeUnit' properties at index= [" + i + "] are not equal:");
                }
            }

            //ReferenceData
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.ExternalId, dto.ReferenceData.ExternalId, "Values for property 'ExternalId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.ExternalSource, dto.ReferenceData.ExternalSource, "Values for property 'ExternalSource' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.TicketNumber, dto.ReferenceData.TicketNumber, "Values for property 'TicketNumber' are not equal:");

            if (expectedTransactionDTO.ReferenceData.DealGroups != null)
            {
                Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups.Count(), dto.ReferenceData.DealGroups.Count(), "Values for property 'DealGroups' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.ReferenceData.DealGroups.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups[i].Name, dto.ReferenceData.DealGroups[i].Name, "'DealGroups." + expectedTransactionDTO.ReferenceData.DealGroups[i].Name + ".Name' properties at index= [" + i + "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups[i].Value, dto.ReferenceData.DealGroups[i].Value, "'DealGroups." + expectedTransactionDTO.ReferenceData.DealGroups[i].Name + ".FeeValue' properties at index= [" + i + "] are not equal:");
                }
            }
            else Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups, dto.ReferenceData.DealGroups, "Values for property 'DealGroups' are not equal:");
            
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.Comment, dto.ReferenceData.Comment, "Values for property 'Comment' are not equal:");
           // Assert.AreEqual(expectedTransactionDTO.ReferenceData.ModificationDateTimeUtc, dto.ReferenceData.ModificationDateTimeUtc, "Values for property 'ModificationDateTimeUtc' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.QuotaRegion, dto.ReferenceData.QuotaRegion, "Values for property 'QuotaRegion' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.RiskValue, dto.ReferenceData.RiskValue, "Values for property 'RiskValue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.Originator, dto.ReferenceData.Originator, "Values for property 'Originator' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.Deliveries, dto.ReferenceData.Deliveries, "Values for property 'Deliveries' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DeclareId, dto.ReferenceData.DeclareId, "Values for property 'DeclareId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.ContractSplitId, dto.ReferenceData.ContractSplitId, "Values for property 'ContractSplitId' are not equal:");

            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DistributionParentTransactionId, dto.ReferenceData.DistributionParentTransactionId, "Values for property 'DistributionParentTransactionId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DistributedQuantity, dto.ReferenceData.DistributedQuantity, "Values for property 'DistributedQuantity' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.OriginalQuantity, dto.ReferenceData.OriginalQuantity, "Values for property 'OriginalQuantity' are not equal:");
           // Assert.AreEqual(expectedTransactionDTO.ReferenceData.ReferringId, dto.ReferenceData.ReferringId, "Values for property 'ReferringId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.CascadingOriginIds, dto.ReferenceData.CascadingOriginIds, "Values for property 'CascadingOriginIds' are not equal:");

            //TransactionWorkFlowDetails
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Authorised, dto.TransactionWorkFlowDetails.Authorised, "Values for property 'Authorised' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Audited, dto.TransactionWorkFlowDetails.Audited, "Values for property 'Audited' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Paid, dto.TransactionWorkFlowDetails.Paid, "Values for property 'Paid' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.ConfirmedByBroker, dto.TransactionWorkFlowDetails.ConfirmedByBroker, "Values for property 'ConfirmedByBroker' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.ConfirmedByCounterparty, dto.TransactionWorkFlowDetails.ConfirmedByCounterparty, "Values for property 'ConfirmedByCounterparty' are not equal:");

            Assert.That(dto.TransactionWorkFlowDetails.TimeStampClearedUtc, Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampClearedUtc).Within(1).Seconds, "Values for property 'TimeStampClearedUTC' are not equal:");
            Assert.That(dto.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc, Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc).Within(1).Seconds, "Values for property 'TimeStampConfirmationBrokerUTC' are not equal: ");
            Assert.That(dto.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc, Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc).Within(1).Seconds, "Values for property 'TimeStampConfirmationCounterPartyUTC' are not equal:");
        
            //Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, dto.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, "Values for property 'TimeStampCounterpartyAuthorised' are not equal:");

            if (expectedTransactionDTO.PropertyGroups != null)
            {
                Assert.AreEqual(expectedTransactionDTO.PropertyGroups.Length,dto.PropertyGroups.Length);

                for (int i = 0; i < expectedTransactionDTO.PropertyGroups.Length; i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Name,dto.PropertyGroups[i].Name,"PropertyGroup Name was not equal");

                    Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties.Length,dto.PropertyGroups[i].Properties.Length,"Differnt number of Properites in : "+expectedTransactionDTO.PropertyGroups[i].Name);

                    if (expectedTransactionDTO.PropertyGroups[i].Properties != null)
                    {
                        for (int j = 0; j < expectedTransactionDTO.PropertyGroups[i].Properties.Length; j++)
                        {
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].Name,dto.PropertyGroups[i].Properties[j].Name,"Property Name is not equal in PropertyGroup"+expectedTransactionDTO.PropertyGroups[i].Name);
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].Value,dto.PropertyGroups[i].Properties[j].Value,"Property FeeValue is not equal in PropertyGroup"+expectedTransactionDTO.PropertyGroups[i].Name);
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].ValueType,dto.PropertyGroups[i].Properties[j].ValueType,"Property ValueType is not equal in PropertyGroup"+expectedTransactionDTO.PropertyGroups[i].Name);
                        }
                    }
                }
            }

           
    }
        
     //  [Test]
        public void GetQATransactionDTOByID()
        {
            int id = 239;

            QaLookUpClient c = new QaLookUpClient();

            QaTransactionDTO dto = c.GetQaTransactionDTO(id);
            Assert.AreEqual(dto.TransactionId, id);
            //Assert.AreEqual(dto.BuySell,QaBuySell.Buy);

            XmlSerializer x = new XmlSerializer(dto.GetType());

            StringWriter textWriter = new StringWriter();

            x.Serialize(textWriter, dto);

            Console.WriteLine(textWriter.ToString());
        }


        private string QATransactionXml(QaTransactionDTO dto)
        {
            XmlSerializer x = new XmlSerializer(dto.GetType());

            StringWriter textWriter = new StringWriter();

            x.Serialize(textWriter, dto);

            return textWriter.ToString();
        }
    }
}

