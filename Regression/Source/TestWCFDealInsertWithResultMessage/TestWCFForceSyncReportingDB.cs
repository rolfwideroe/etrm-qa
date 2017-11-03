using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestWCFDealInsertWithResultMessage
{
    class TestWCFForceSyncReportingDB
    {
        private const string TestFilesPath = "TestFilesForceSync\\";
        private static readonly IEnumerable<string> TestFiles = TestCasesFileEnumeratorByFolder.TestCaseFilesFiltred(TestFilesPath);

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFiles")]
        public void UpdateDealForceSync(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesPath + testFile);
            
            //inserting deals
            int trID = InsertDeal(testFilePath);
            Assert.True(trID > 0, "Inserting deal failed");

            //update transaction in reporting db
            ExecuteUpdateQuery(trID);

            //force sync
            RunForceSycnJob();

            //Assert with expected QATransactionDTO
            XmlDocument doc = new XmlDocument();
            doc.Load(testFilePath); 
            XmlNodeList assertValue = doc.GetElementsByTagName("AssertQaTransactionDTO");
            Assert.AreEqual(1, assertValue.Count, "Test case doesn't have 'AssertQaTransactionDTO' XML-node.");

            if (assertValue[0].InnerText.ToUpper() == "TRUE")
            {
                XmlNodeList expectedDtoNode = doc.GetElementsByTagName("QaTransactionDTO");
                if (expectedDtoNode.Count > 0)
                {
                    Thread.Sleep(3000);
                    TestWCFDealInsertReportingDB.CompareTransactionDto(testFilePath, trID);
                }
                else
                {
                    Assert.Fail("Test case doesn't have 'AssertQaTransactionDTO' XML-node.");
                }
            }

        }


        public int InsertDeal(string filePath)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList dealNode = doc.GetElementsByTagName("TestData");
            if (dealNode.Count != 1)
            {
                Assert.Fail("Missing TestData");
            }

            string result = "";

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            try
            {
                result = dealServiceClient.ImportDeal(dealNode[0].InnerXml);
                // Console.WriteLine(dealNode[0].InnerXml);
               // Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            //create and read XML with ImportDeal's results
            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(result);

            XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
            string dealinsertResult = resultNode[0].InnerText;
            XmlNode xmlNode = resultXml.GetElementsByTagName("Message").Item(0);
            if (xmlNode != null)
                Assert.AreEqual("SUCCESS", dealinsertResult.ToUpper(), xmlNode.InnerText);

            XmlNodeList IdNode = resultXml.GetElementsByTagName("TransactionId");
            int insertedTransactionID = Convert.ToInt32(IdNode[0].InnerText);

            return insertedTransactionID;
        }


        //   [Test]
        public void ExecuteUpdateQuery(int trId)
        {
           string sqlCommand = string.Format(
                  @"DECLARE @ColNames TABLE (ColumnName varchar (50))
                    insert into @ColNames
	                select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = 'ContractExports' and DATA_TYPE = 'datetime'

                    declare @tableCursor cursor,      
                            @columnName varchar(50)

                    set @tableCursor = cursor for select * from @ColNames

                    open @tableCursor
                    fetch next from @tableCursor into @columnName
                    while(@@fetch_status = 0)
                    begin                       
                        declare @sql varchar(max)
                        set @sql = 'update ContractExports set '+@columnName+'=''2011-11-01'' where ContractId={0}'
	                    exec (@sql)

                        fetch next from @tableCursor into @columnName
                    end
                    close @tableCursor
                    deallocate @tableCursor;
                    update ContractExports set Audited= 1, Currency = 'USD', Comment='destoing_data_update', BrokerFeeFixed=1 where ContractId = {0};
                    update ContractExportsCustomFields set PropertyCustom1 = 'removing value' where ContractExportId = {0}"
                   , trId);

            QaDao.UpdateReportingDBRecord("ReportingDatabase", sqlCommand);
        }

        //    [Test]
        public void RunForceSycnJob()
        {
            Dictionary<string, string> optionalParams = new Dictionary<string, string>();
            optionalParams.Add("ReexportAll", "true");
            JobAPI.ExecuteAndAssertJob(12, optionalParams, 300);
        }

    }
}
