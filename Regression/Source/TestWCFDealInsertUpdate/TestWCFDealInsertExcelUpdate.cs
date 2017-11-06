using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using ElvizTestUtils.DealServiceReference;
using TimeSeries = ElvizTestUtils.LookUpServiceReference.TimeSeries;
using TimeSeriesValue = ElvizTestUtils.LookUpServiceReference.TimeSeriesValue;


namespace TestWCFDealInsertUpdate
{
    [TestFixture]
    public class TestWCFDealInsertExcelUpdate
    {
    
        [TestFixtureSetUp]
        public void Setup()
        {
            //if (ElvizInstallationUtility.PamEnabled)
            //{
            //    ElvizInstallationUtility.PamEnabled = false;
            //}
        }

        private static readonly IEnumerable<string> TestFilesDealInsertUpdateExcel = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\ExcelUpdateTests");

        [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesDealInsertUpdateExcel")]
        public void TestWcfDealInsertUpdateTestFromExcel(string testFile)
        {
     

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(),"TestFiles\\ExcelUpdateTests\\" + testFile);

            DealInsertUpdateTestCase testCase = TestXmlTool.Deserialize<DealInsertUpdateTestCase>(testFilePath);  
         

            if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");

            if(string.IsNullOrEmpty(testCase.UpdateExcel.FileName)) Assert.Fail("Wrong or missing UpdateExcel FileName Node");

            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(testCase.InsertXml);  

            bool isBulkInsert = false;
            XmlNodeList bulkNode = insertXmlDocument.GetElementsByTagName("BulkDealInsert");

            if (bulkNode.Count > 0) 
                isBulkInsert = true;


            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();
            
            string expectedType = testCase.ExpectedResult.ExpectedType;


            IDictionary<string,int> transIds = InsertXmlReturnTransId(testCase.InsertXml, dealServiceClient,isBulkInsert);
            if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");

            if(testCase.ExpectedResult.ExpectedTimeSeriesSets != null && transIds.Count!=testCase.ExpectedResult.ExpectedTimeSeriesSets.Count()) Assert.Fail("Inserted Transactions did not match expected Timeseriessets");

            ExecuteUpdate(testCase.UpdateExcel.FileName, dealServiceClient, testCase.ExpectedResult);

            if (expectedType == "TimeSeries")
            {
                foreach (KeyValuePair<string, int> keyValuePair in transIds)
                {
                    string extId = keyValuePair.Key;
                    int transId = keyValuePair.Value;
                   
                    TransactionDTO dto = lookupServiceClient.GetTransactionsByIds(new []{transId})[0];

                    ExpectedTimeSeriesSet expectedTimeSeriesSet =
                        testCase.ExpectedResult.ExpectedTimeSeriesSets.Single(x => x.ExternalId == extId);

                    AssertTimeSeriesSet(dto, expectedTimeSeriesSet);
                }
                
            }

            //if (expectedType == "ErrorMessage") //|| (updateresult == "Failure"))
            //{
                
            //    //XmlNodeList nodeList = testCaseXml.GetElementsByTagName("ErrorMessage");

            //        string errormessage =testCase.ExpectedResult.ErrorMessage;

            //        //string formatedErrorMessage = Regex.Replace(errormessage, @"\t|\n|\r", "");
            //        //string formatedResult = Regex.Replace(updateResultMessage, @"\t|\n|\r", "");
            //        //Console.WriteLine("server returns: " + updateResultMessage);
            //        //Assert.AreEqual(formatedErrorMessage, formatedResult,
            //        //    "Expected is : " + errormessage + " \n But Was: " + updateResultMessage);
            //    Assert.Contains(errormessage);
               
            //    else Assert.Fail("Test failed with error message: " + updateResultMessage);
            //    return false;
            //}
        }

        private void ExecuteUpdate(string updateFileName, IDealService dealServiceClient, ExpectedResult expectedResult)
        {
            try
            {
                string fileName = updateFileName;
                string testFilePath = Path.Combine(Directory.GetCurrentDirectory(),"TestFiles\\ExcelUpdateTests\\ExcelFiles\\" + fileName);
                // Get content of Excel file
                MemoryStream ms = new MemoryStream();
                byte[] filecontent;

                using (FileStream fs = new FileStream(testFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Write content of your memory stream into file stream
                    fs.CopyTo(ms);
                    filecontent = ms.ToArray();

                }
                string dealResult = "";
                try
                {
                    dealResult = dealServiceClient.PartialUpdateImportDeal(filecontent, "ImportFormatNew.xlsx");
                }
                catch (Exception ex)
                {

                    dealResult = ex.Message;
                }
                

                if ((expectedResult.ExpectedType == "ErrorMessage") || (dealResult == "Failure"))
                {                                          
                        string formattedErrorMessage = Regex.Replace(expectedResult.ErrorMessage, @"\t|\n|\r", "");
                        string formattedResult = Regex.Replace(dealResult, @"\t|\n|\r", "");
                        //Console.WriteLine("server returns: " + updateResultMessage);
                        Assert.AreEqual(formattedErrorMessage, formattedResult,
                            "Expected is : " + formattedErrorMessage + " \n But Was: " + dealResult);
                    return;
                }

                if (!dealResult.Contains("<Result>Success</Result>")) Assert.Fail(dealResult);
                
                 
                   
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                
            }
       
        }

        //private static T NodeToObject<T>(XmlNode xmlNode)
        //{
        //    T result;
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        StreamWriter writer = new StreamWriter(stream);
        //        writer.Write(xmlNode.OuterXml);
        //        writer.Flush();
        //        stream.Position = 0;

        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

        //        result = (T)xmlSerializer.Deserialize(stream);
        //    }
        //    return result;
        //}

 
        private static void AssertTimeSeriesSet(TransactionDTO dto, ExpectedTimeSeriesSet set)
        {
            TimeSeries[] actualTimeSeries = dto.TimeSeriesSet;
            ExpectedTimeSeries[] expectedTimeSeries = set.ExpectedTimeSeries;

            Assert.AreEqual(expectedTimeSeries.Count(),actualTimeSeries.Count(),"Number of Timeseries did not match for : "+set.ExternalId);

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

                    Assert.AreEqual(expectedVal.FromDateTime, actaulVal.FromDateTime);
                    Assert.AreEqual(expectedVal.UntilDateTime, actaulVal.UntilDateTime);
                    Assert.AreEqual(expectedVal.UtcFromDateTime, actaulVal.UtcFromDateTime);
                    Assert.AreEqual(expectedVal.UtcUntilDateTime, actaulVal.UtcUntilDateTime);

                    string errMsg = "Time series " + actual.TimeSeriesType.Name + ": Error in period " + expectedVal.FromDateTime + " - " + expectedVal.UntilDateTime;
                    Assert.AreEqual(expectedVal.Value, actaulVal.Value, errMsg);
                }
            }
        }
       

        public Dictionary<string,int> InsertXmlReturnTransId(string insertXml, IDealService dealServiceClient,bool isBulkInsert)
        {
            Dictionary<string,int> insertedExtIdsAndInternalIds = new Dictionary<string, int>(); 

            try
            {               

                string insertResultXml = dealServiceClient.ImportDeal(insertXml);

                XmlDocument resultXml = new XmlDocument();
                resultXml.LoadXml(insertResultXml);

                XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
                XmlNodeList transIdNode = resultXml.GetElementsByTagName("TransactionId");
                XmlNode extIdNode = resultXml.SelectSingleNode("/DealResult/Details/Transaction/TransactionDetails/ExternalTransactionId");

                if (resultNode[0].InnerXml == "Success")
                {
                    if (isBulkInsert)
                    {
                        if (extIdNode == null) throw new ArgumentException("Insert was success and Bulk Insert, but returned no external id");

                        string[] extIds = Regex.Split(extIdNode.InnerText, "; ");

                        QaDao utility = new QaDao();

                        int[] transIds = utility.GetOriginalTransactionIdsFromExternalIds(extIds);

                        for (int i = 0; i < extIds.Length; i++)
                        {
                            string extId = extIds[i];
                            int internalId = transIds[i];

                            insertedExtIdsAndInternalIds.Add(extId,internalId);
                        }

                        

                    }
                    else
                    {
                         string transIdString = (transIdNode[0].InnerText);

                         int transId;
                        int.TryParse(transIdString, out transId);
                        if(transId==0) throw new ArgumentException("Insert was sucess and not bulk insert but did not return a transaction id");

                        XmlDocument insertXmlDoc=new XmlDocument();
                        insertXmlDoc.LoadXml(insertXml);

                        XmlNode d = insertXmlDoc.SelectSingleNode("DealInsert/Transaction/ReferenceData/ExternalId");
                        string extId = d.InnerText;

                        insertedExtIdsAndInternalIds.Add(extId,transId);
                    }
                  
                }
      
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }



            return insertedExtIdsAndInternalIds;
        }
    }
}
