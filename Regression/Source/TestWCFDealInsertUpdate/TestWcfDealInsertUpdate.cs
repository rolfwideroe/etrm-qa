using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using TimeSeries = ElvizTestUtils.LookUpServiceReference.TimeSeries;
using TimeSeriesValue = ElvizTestUtils.LookUpServiceReference.TimeSeriesValue;

namespace TestWCFDealInsertUpdate
{
    
        [TestFixture]
         public class TestWcfDealInsertUpdate
        {
           
            [SetUp]
            public void Setup()
            {
                if (ConfigurationTool.PamEnabled)
                {
                    ConfigurationTool.PamEnabled = false;
                }
            }
      
            private static readonly IEnumerable<string> TestFilesDealInsertUpdate = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesDealInsertUpdate")] 
            public void TestWcfDealInsertUpdateTestFromXmlFile(string testFile)
            {
                string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);

                DealInsertUpdateTestCase testCase = TestXmlTool.Deserialize<DealInsertUpdateTestCase>(testFilePath);

                if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");

                XmlDocument insertXmlDocument = new XmlDocument();
                insertXmlDocument.LoadXml(testCase.InsertXml);

                bool isBulkInsert = false;
                XmlNodeList bulkNode = insertXmlDocument.GetElementsByTagName("BulkDealInsert");

                if (bulkNode.Count > 0)
                    isBulkInsert = true;
                
                IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

                ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

                string expectedType = testCase.ExpectedResult.ExpectedType;

                IDictionary<string, int> transIds = DealInsertTools.InsertXmlReturnTransId(testCase.InsertXml, dealServiceClient, isBulkInsert);
                if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");
               // foreach (var transId in transIds) Console.WriteLine("Id " + transId.Key + " = "+  transId.FeeValue);

                if (!ExecuteUpdate(testCase, dealServiceClient)) return;

                if (expectedType == "TimeSeries")
                {
                    foreach (KeyValuePair<string, int> keyValuePair in transIds)
                    {
                        string extId = keyValuePair.Key;
                        int transId = keyValuePair.Value;

                        TransactionDTO dto = lookupServiceClient.GetTransactionsByIds(new[] { transId })[0];

                        ExpectedTimeSeriesSet expectedTimeSeriesSet =
                            testCase.ExpectedResult.ExpectedTimeSeriesSets.Single(x => x.ExternalId == extId);

                        AssertTimeSeriesSet(dto, expectedTimeSeriesSet);
                    }

                }
            }

            private static void AssertTimeSeriesSet(TransactionDTO dto, ExpectedTimeSeriesSet set)
            {
                TimeSeries[] actualTimeSeries = dto.TimeSeriesSet;
                ExpectedTimeSeries[] expectedTimeSeries = set.ExpectedTimeSeries;

                Assert.AreEqual(expectedTimeSeries.Count(), actualTimeSeries.Count(), "Number of Timeseries did not match for : " + set.ExternalId);

                for (int i = 0; i < expectedTimeSeries.Count(); i++)
                {
                    TimeSeries actual = actualTimeSeries[i];
                    ExpectedTimeSeries expected = expectedTimeSeries[i];

                    ExpectedTimeSeriesValue[] expectedValues = expected.ExpectedTimeSeriesValues;
                    TimeSeriesValue[] actualValues = actual.TimeSeriesValues;

                    Assert.AreEqual(expectedValues.Count(), actualValues.Count(), "Number of Timeseries values did not match for : " + set.ExternalId);

                    for (int j = 0; j < expectedValues.Count(); j++)
                    {
                        ExpectedTimeSeriesValue expectedVal = expectedValues[j];
                        TimeSeriesValue actaulVal = actualValues[j];

                        string errPeriods = "Time series " + actual.TimeSeriesType.Name + ": ExpectedTimeSeriesValue row =" + (j+1) + "; attribude name =";

                        Assert.AreEqual(expectedVal.FromDateTime, actaulVal.FromDateTime, errPeriods + "FromDateTime");
                        Assert.AreEqual(expectedVal.UntilDateTime, actaulVal.UntilDateTime, errPeriods + "UntilDateTime");
                        Assert.AreEqual(expectedVal.UtcFromDateTime, actaulVal.UtcFromDateTime, errPeriods + "UtcFromDateTime");
                       Assert.AreEqual(expectedVal.UtcUntilDateTime, actaulVal.UtcUntilDateTime, errPeriods + "UtcUntilDateTime");

                        string errMsg = "Time series " + actual.TimeSeriesType.Name + ": Error in period " + expectedVal.FromDateTime + " - " + expectedVal.UntilDateTime;

                        if (expectedVal.Value.HasValue && actaulVal.Value.HasValue)
                        {
                            Assert.AreEqual(Math.Round((double) expectedVal.Value,10), Math.Round((double) actaulVal.Value,10), errMsg);                            
                        }
                        else
                        {
                            Assert.AreEqual(expectedVal.Value, actaulVal.Value, errMsg);                            
                        }
                    }
                }
            }

            private static readonly IEnumerable<string> TestFilesReportDate = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\ReportDateTests");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesReportDate")]
            public void TestWcfDealInsertUpdateTestFromXmlFileWithReportDate(string testFile)
            {
               
                string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\ReportDateTests\\" + testFile);
                
                DealInsertUpdateTestCase testCase = TestXmlTool.Deserialize<DealInsertUpdateTestCase>(testFilePath);

                if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");

                XmlDocument insertXmlDocument = new XmlDocument();
                insertXmlDocument.LoadXml(testCase.InsertXml);

                bool isBulkInsert = false;
                XmlNodeList bulkNode = insertXmlDocument.GetElementsByTagName("BulkDealInsert");

                if (bulkNode.Count > 0)
                    isBulkInsert = true;
                
                if ((testCase.ReportDate.Value == null)) Assert.Fail("Wrong or missing ReportDate Node");
                DateTime reportDate = Convert.ToDateTime(testCase.ReportDate.Value, CultureInfo.InvariantCulture);
                
                IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
                ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

                IDictionary<string, int> transIds = DealInsertTools.InsertXmlReturnTransId(testCase.InsertXml, dealServiceClient, isBulkInsert);
                if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");

                if (!ExecuteUpdate(testCase, dealServiceClient)) return;

                if (testCase.ExpectedResult.ExpectedType == "TimeSeries")
                {
                    foreach (KeyValuePair<string, int> keyValuePair in transIds)
                    {
                        string extId = keyValuePair.Key;
                        int transId = keyValuePair.Value;

                        TransactionDTO dto = lookupServiceClient.GetTransactionsByIdsAndReportDate(new[] { transId }, reportDate)[0];

                        ExpectedTimeSeriesSet expectedTimeSeriesSet =
                            testCase.ExpectedResult.ExpectedTimeSeriesSets.Single(x => x.ExternalId == extId);

                        AssertTimeSeriesSet(dto, expectedTimeSeriesSet);
                    }
               }
            }

            private static readonly IEnumerable<string> TestFilesVersionExtract = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\VersionExtractTests");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesVersionExtract")]
            public void TestWcfDealInsertUpdateTestFromXmlFileExtractVersion(string testFile)
            {
                
                string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\VersionExtractTests\\" + testFile);

                DealInsertUpdateTestCase testCase = TestXmlTool.Deserialize<DealInsertUpdateTestCase>(testFilePath);

                if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");

                XmlDocument insertXmlDocument = new XmlDocument();
                insertXmlDocument.LoadXml(testCase.InsertXml);

                bool isBulkInsert = false;
                XmlNodeList bulkNode = insertXmlDocument.GetElementsByTagName("BulkDealInsert");

                if (bulkNode.Count > 0)
                    isBulkInsert = true;

                if ((testCase.ReportDate.Value == null)) Assert.Fail("Wrong or missing ReportDate Node");
                DateTime reportDate = Convert.ToDateTime(testCase.ReportDate.Value, CultureInfo.InvariantCulture);

                IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
                ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

                IDictionary<string, int> transIds = DealInsertTools.InsertXmlReturnTransId(testCase.InsertXml, dealServiceClient, isBulkInsert);
                if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");

                if (!ExecuteUpdate(testCase, dealServiceClient)) return;


                if (testCase.ExpectedResult.ExpectedType == "Versions")
                {
                   foreach (KeyValuePair<string, int> keyValuePair in transIds)
                   {
                       string extId = keyValuePair.Key;
                       int transId = keyValuePair.Value;

                       //TransactionDTO dto = lookupServiceClient.GetTransactionsByIdsAndReportDate(new[] { transId }, reportDate)[0];

                       ExpectedVersions expectedVersions = testCase.ExpectedResult.ExpectedVersions.Single(x => x.ExternalId == extId);

                       AssertVersions(expectedVersions, lookupServiceClient, transId);
                    }
                }
         

            }

            private static void AssertVersions(ExpectedVersions expectedVersions, ILookupService lookupServiceClient, int transId)
            {
                
                foreach (ExpectedVersion version in expectedVersions.ExpectedVersion)
                {
                    TransactionDTO dto = null;
                    if (version.IsLatestVersion)
                    {
                        dto = lookupServiceClient.GetTransactionsByIds(new[] { transId })[0];
                    }

                    if (!version.IsLatestVersion)
                    {
                        dto =
                            lookupServiceClient.GetTransactionsByIdsAndReportDate(new[] { transId },
                                version.EffectiveDate)[0];
                    }

                    if (dto != null)
                    {
                        Assert.NotNull(dto.Version);
                        Assert.AreEqual(version.ExpectedVersionDate, dto.Version.EffectiveDate, "ExpectedVersionDate are not equal TransactionDTO.Version.EffectiveDate. TransactionId = " + dto.TransactionId);

                        if (version.ExpectedTimeSeriesSet != null)
                        {
                           AssertVersionTimeSeriesSet(dto, version.ExpectedTimeSeriesSet, version.ExpectedVersionDate.ToString());
                        }

                    }

                }
                
            }

            private static void AssertVersionTimeSeriesSet(TransactionDTO dto, ExpectedTimeSeriesSet set, string date)
            {
                TimeSeries[] actualTimeSeries = dto.TimeSeriesSet;
                ExpectedTimeSeries[] expectedTimeSeries = set.ExpectedTimeSeries;

                string errormsg =String.Format("Error in version set with EffectiveDate = {0}.\n", date.Substring(0,10));

                Assert.AreEqual(expectedTimeSeries.Count(), actualTimeSeries.Count(), errormsg + "Number of Timeseries did not match for : " + set.ExternalId);

                for (int i = 0; i < expectedTimeSeries.Count(); i++)
                {
                    TimeSeries actual = actualTimeSeries[i];
                    ExpectedTimeSeries expected = expectedTimeSeries[i];

                    Assert.AreEqual(expected.TimeSeriesTypeName, actual.TimeSeriesType.Name, errormsg + "TimeSeriesTypeName are not equal: ");
                    Assert.AreEqual(expected.TimezoneName, actual.TimezoneName, errormsg + "TimeZoneName are not equal: "); 

                    ExpectedTimeSeriesValue[] expectedValues = expected.ExpectedTimeSeriesValues;
                    TimeSeriesValue[] actualValues = actual.TimeSeriesValues;

                    Assert.AreEqual(expectedValues.Count(), actualValues.Count(), errormsg+ "Number of Timeseries values did not match for : " + set.ExternalId);

                    for (int j = 0; j < expectedValues.Count(); j++)
                    {
                        ExpectedTimeSeriesValue expectedVal = expectedValues[j];
                        TimeSeriesValue actaulVal = actualValues[j];

                        string errPeriods = errormsg + "Time series " + actual.TimeSeriesType.Name + ": ExpectedTimeSeriesValue row =" + (j + 1) + "; attribude name =";

                        Assert.AreEqual(expectedVal.FromDateTime, actaulVal.FromDateTime, errPeriods + "FromDateTime");
                        Assert.AreEqual(expectedVal.UntilDateTime, actaulVal.UntilDateTime, errPeriods + "UntilDateTime");
                        Assert.AreEqual(expectedVal.UtcFromDateTime, actaulVal.UtcFromDateTime, errPeriods + "UtcFromDateTime");
                        Assert.AreEqual(expectedVal.UtcUntilDateTime, actaulVal.UtcUntilDateTime, errPeriods + "UtcUntilDateTime");

                        string errMsg = errormsg+ "Time series " + actual.TimeSeriesType.Name + ": Check period " + expectedVal.FromDateTime + " - " + expectedVal.UntilDateTime;
                        Assert.AreEqual(expectedVal.Value, actaulVal.Value, errMsg);
                    }
                }
            }


            //update
            private static bool ExecuteUpdate(DealInsertUpdateTestCase testCase, IDealService dealServiceClient)
            {
                string updateResultMessage = "";
                string updateresult = "Success";

                //Execute update part of xml
                try
                {
                    string updateMessage = dealServiceClient.ImportDeal(testCase.UpdateXml);

                    XmlDocument resultXml = new XmlDocument();
                    resultXml.LoadXml(updateMessage);


                    XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
                    XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");

                    if (resultNode[0].InnerXml != "Success")
                    {
                        updateResultMessage = messageNode[0].InnerText;
                        updateresult = resultNode[0].InnerXml;
                    }
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.ToString());
                }


                if ((testCase.ExpectedResult.ExpectedType == "ErrorMessage") || (updateresult == "Failure"))
                {
                    string expectedError = testCase.ExpectedResult.ErrorMessage;
                    string formatedErrorMessage = expectedError ==null? "No errors" : Regex.Replace(expectedError, @"\t|\n|\r", "");
                    string formatedResult = Regex.Replace(updateResultMessage, @"\t|\n|\r", "");
                    
                    Assert.AreEqual(formatedErrorMessage, formatedResult,
                            "Expected is : " + expectedError + " \n But Was: " + updateResultMessage);
                    return false;
                }
                return true;
            }


           

			
        }

}
