using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;
using System.Data.SqlClient;

namespace TestWCFDealInsertUpdateRevision
{
    
     [TestFixture]
     public class TestWcfDealInsertUpdateRevision
        {
      
            [OneTimeSetUp]
                public void Setup()
                {
                    if (ConfigurationTool.PamEnabled)
                    {
                        ConfigurationTool.PamEnabled = false;
                    }

                    Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            }

     
            private static readonly IEnumerable<string> TestFilesUpdateRevisiont = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesUpdateRevisiont")]
            public void TestWcfDealInsertUpdateRevisionTestFromXmlFile(string testFile)
            {

                //Console.WriteLine("Current Thread Culture/UI Culture: {0}/{1}",
                //                  Thread.CurrentThread.CurrentCulture.Name,
                //                  Thread.CurrentThread.CurrentUICulture.Name);
                
                XmlDocument testCaseXml = new XmlDocument();

                string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
                testCaseXml.Load(testFilePath);      // input XML

                XmlNodeList insertxmlNode = testCaseXml.GetElementsByTagName("InsertXml");
                if (insertxmlNode.Count !=1) Assert.Fail("Wrong or Missing XmlInsertNode");

                XmlNodeList updateXmlNode = testCaseXml.GetElementsByTagName("UpdateXml");
                if (updateXmlNode.Count != 1) Assert.Fail("Wrong or Missing UpdateXml Node");

                XmlNodeList update1XmlNode = testCaseXml.GetElementsByTagName("Update1Xml");
                if (update1XmlNode.Count != 1) Assert.Fail("Wrong or Missing Update1Xml Node");

                XmlNodeList expectedResultNode = testCaseXml.GetElementsByTagName("ExpectedResult");
                if(expectedResultNode.Count!=1) Assert.Fail("Wrong or Missing Expected Result");

               IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

               ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();
				
			   string expectedType = expectedResultNode[0].Attributes["ExpectedType"].Value;

               //Always expect innsert to be successfull
               int transId = InsertXmlReturnTransId(insertxmlNode, dealServiceClient, "Success", testCaseXml);
				if (transId == 0) return;

               if (!ExecuteUpdate(updateXmlNode, dealServiceClient, expectedType, testCaseXml)) return;

               if (!ExecuteUpdate(update1XmlNode, dealServiceClient, expectedType, testCaseXml)) return;


	            if (expectedType == "TimeSeries")
	            {
		            //get the revision#1 value and compare
                    DataTable revisionValue = LookUpRevisionValue(transId);
                    AssertTimeSeriesValues(testCaseXml, revisionValue);
	            }
            }

	        private static void AssertTimeSeriesValues(XmlDocument testCaseXml, DataTable RevisionValues)
	        {
		        XmlNodeList expectedTimeSeriesNodeList = testCaseXml.GetElementsByTagName("ExpectedTimeSeriesSet");
		        XmlNode expectedTimeSeriesNode = expectedTimeSeriesNodeList[0];
		        XmlSerializer xmlSerializer = new XmlSerializer(typeof (ExpectedTimeSeriesSet));

		        ExpectedTimeSeriesSet set;
		        using (MemoryStream memoryStream = new MemoryStream())
		        {
			        XmlDocument docum = new XmlDocument();
			        docum.AppendChild(docum.ImportNode(expectedTimeSeriesNode, true));
			        docum.Save(memoryStream);
			        memoryStream.Position = 0;
			        XmlReader reader = XmlReader.Create(memoryStream);

			        set = (ExpectedTimeSeriesSet) xmlSerializer.Deserialize(reader);
		        }

		        ExpectedTimeSeries[] expectedTimeSeries = set.ExpectedTimeSeries;


		        for (int i = 0; i < expectedTimeSeries.Count(); i++)
		        {
			        ExpectedTimeSeries expected = expectedTimeSeries[i];

			        ExpectedTimeSeriesValue[] expectedValues = expected.ExpectedTimeSeriesValues;
		            
			        for (int j = 0; j < expectedValues.Count(); j++)
			        {
				        ExpectedTimeSeriesValue expectedVal = expectedValues[j];
                        
                        foreach (DataRow row in RevisionValues.Rows)
                        {
				            string errMsg = "Time series " + "ForecastedVolume" + ": Error in period " + expectedVal.FromDateTime + " - " + expectedVal.UntilDateTime;
                            Assert.AreEqual(expectedVal.Value, row["Value"], errMsg);
                            break;
                        }
			            
			        }
		           
		        }
	        }

	        private static bool ExecuteUpdate(XmlNodeList updateXmlNode, IDealService dealServiceClient, string expectedType, XmlDocument testCaseXml)
	        {
		        string updateResultMessage = "";
		        string updateresult = "Success";

		        //Execute update part of xml
		        try
		        {
			        string updateXml = updateXmlNode[0].InnerXml;

			        string updateMessage = dealServiceClient.ImportDeal(updateXml);

			        XmlDocument resultXml = new XmlDocument();
			        resultXml.LoadXml(updateMessage);


			        XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
			        XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");

			        if (resultNode[0].InnerXml != "Success")
			        {
				        updateResultMessage = messageNode[0].InnerXml;
				        updateresult = resultNode[0].InnerXml;
			        }
		        }
		        catch (Exception ex)
		        {
			        Assert.Fail(ex.ToString());
		        }


		        if ((expectedType == "ErrorMessage") || (updateresult == "Failure"))
		        {
			        XmlNodeList nodeList = testCaseXml.GetElementsByTagName("ErrorMessage");
			        if (nodeList.Count > 0)
			        {
				        string errormessage = nodeList[0].InnerXml;

				        string formatedErrorMessage = Regex.Replace(errormessage, @"\t|\n|\r", "");
				        string formatedResult = Regex.Replace(updateResultMessage, @"\t|\n|\r", "");
				        //Console.WriteLine("server returns: " + updateResultMessage);
				        Assert.AreEqual(formatedErrorMessage, formatedResult,
					        "Expected is : " + errormessage + " \n But Was: " + updateResultMessage);
			        }
			        else Assert.Fail("Test failed with error message: " + updateResultMessage);
			        return false;
		        }
		        return true;
	        }

			private static int InsertXmlReturnTransId(XmlNodeList insertxmlNode, IDealService dealServiceClient, string expectedType, XmlDocument testCaseXml)
	        {
		        int transId = 0;
				string insertResultMessage = "";
				string insertResult = "Success";

		        try
		        {
			        string insertNodeFromXml = insertxmlNode[0].InnerXml;

			        string insertResultXml = dealServiceClient.ImportDeal(insertNodeFromXml);

			        XmlDocument resultXml = new XmlDocument();
			        resultXml.LoadXml(insertResultXml);

			        XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
			        XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");
			        XmlNodeList transIdNode = resultXml.GetElementsByTagName("TransactionId");

			        if (resultNode[0].InnerXml == "Success")
			        {
						transId = int.Parse(transIdNode[0].InnerText);    
			        }
					else
					{
						insertResultMessage = messageNode[0].InnerXml;
						insertResult = resultNode[0].InnerXml;
					}
			        
		        }
		        catch (Exception ex)
		        {
			        Assert.Fail(ex.ToString());
		        }

				if ((expectedType == "ErrorMessage") || (insertResult == "Failure"))
				{
					XmlNodeList nodeList = testCaseXml.GetElementsByTagName("ErrorMessage");
					if (nodeList.Count > 0)
					{
						string errormessage = nodeList[0].InnerXml;

						string formattedErrorMessage = Regex.Replace(errormessage, @"\t|\n|\r", "");
						string formattedResult = Regex.Replace(insertResultMessage, @"\t|\n|\r", "");
						//Console.WriteLine("server returns: " + updateResultMessage);
						Assert.AreEqual(formattedErrorMessage, formattedResult,
							"Expected is : " + errormessage + " \n But Was: " + insertResultMessage);
					}
					else Assert.Fail("Test failed with error message: " + insertResultMessage);
					return 0;
				}
				
		        return transId;
	        }

            private DataTable LookUpRevisionValue(int transactionId)
            {
                DataTable tableTimeSeriesIdentifier = new DataTable();
                DataTable tableTimeSeriesVersionId = new DataTable();
                DataTable tableTimeSeriesRevisionId = new DataTable();
                DataTable tableRevisions = new DataTable();

                /*
                sqlServerName = ElvizInstallationUtility.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\Servers", "Default");
                databaseUsername = ElvizInstallationUtility.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + sqlServerName, "DefaultUser");
                databasePassword = ElvizInstallationUtility.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + sqlServerName, "DefaultPw");
                ecmDatabaseName = ElvizInstallationUtility.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + sqlServerName, "VizEcm");
                */
                string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
                string ecmDatabaseName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
             

                //foreach (XmlNode node in configurationDetailsXml)
                //{
                //    sqlServerName = node["ServerName"].InnerText;
                //    ecmDatabaseName = node["DataBaseName"].InnerText;
                //    databaseUsername = node["DataBaseUserName"].InnerText;
                //    databasePassword = node["DataBasePassword"].InnerText;
                //}
                SqlConnection myConnection = new SqlConnection(string.Format("Data Source={0};Initial Catalog={1};Trusted_Connection=True;", sqlServerName,ecmDatabaseName));
                    
                  
                
                try
                {
                    myConnection.Open();
                    SqlCommand myCommand;
                    SqlDataAdapter adapter;
                    string timeSeriesIdentifier = String.Empty;
                    myCommand = new SqlCommand("select TimeSeriesIdentifier from TransactionTimeSeriesLinks h where h.TransactionId="+ transactionId +" and TimeSeriesRepresentationTypeId=1", myConnection);
                    adapter = new SqlDataAdapter(myCommand);
                    adapter.Fill(tableTimeSeriesIdentifier);
                    if (tableTimeSeriesIdentifier.Rows.Count == 1)
                    {
                        foreach (DataRow row in tableTimeSeriesIdentifier.Rows)
                        {
                           timeSeriesIdentifier = row["TimeSeriesIdentifier"].ToString();
                        }
                    }
                    else
                    {
                        Assert.Fail("Test failed with: No TimeSeriesIdentifier for ForecastedVolume for the transaction.");
                    }

                    string timeSeriesVersionId = String.Empty;
                    myCommand = new SqlCommand("select TimeSeriesVersionId from TimeSeriesVersions v where v.TimeSeriesId='" + timeSeriesIdentifier + "'", myConnection);
                    adapter = new SqlDataAdapter(myCommand);
                    adapter.Fill(tableTimeSeriesVersionId);
                    if (tableTimeSeriesVersionId.Rows.Count == 1)
                    {
                        foreach (DataRow row in tableTimeSeriesVersionId.Rows)
                        {
                            timeSeriesVersionId = row["TimeSeriesVersionId"].ToString();
                        }
                    }
                    else
                    {
                        Assert.Fail("Test failed: No TimeSeriesVersionId for ForecastedVolume for the transaction.");
                    }

                    string timeSeriesRevisionId = String.Empty;
                    myCommand = new SqlCommand("select * from TimeSeriesVersionRevisions where TimeSeriesVersionId='" + timeSeriesVersionId + "' order by TimeSeriesVersionId", myConnection);
                    adapter = new SqlDataAdapter(myCommand);
                    adapter.Fill(tableTimeSeriesRevisionId);
                    if (tableTimeSeriesRevisionId.Rows.Count > 1)
                    {
                        bool secondRow = false;
                        foreach (DataRow row in tableTimeSeriesRevisionId.Rows)
                        {
                            if (secondRow==true)
                            {
                                timeSeriesRevisionId = row["TimeSeriesRevisionId"].ToString();    
                                break;
                            }
                            secondRow = true;
                        }
                    }
                    else
                    {
                        Assert.Fail("Test failed: Correct revision not found.");
                    }

                    myCommand = new SqlCommand("select * from TimeSeriesFixedResolutionValuesHistory hy where hy.FromTimeSeriesRevisionId=" + timeSeriesRevisionId + "order by SequenceNumber", myConnection);
                    adapter = new SqlDataAdapter(myCommand);
                    adapter.Fill(tableRevisions);
                    
                }
                catch (Exception e)
                {
                    Assert.Fail(e.ToString());
                }
                myConnection.Close();
                return tableRevisions;
            }
        }
    


}
