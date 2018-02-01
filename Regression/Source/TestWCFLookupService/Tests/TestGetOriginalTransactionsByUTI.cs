using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using TestWCFLookupService.TestCases;

namespace TestWCFLookupService.Tests
{
    [TestFixture]
    class TestGetOriginalTransactionsByUTI
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        private const string TestFilePath = "Testfiles\\TransactionsByUTI\\";

        private static readonly IEnumerable<string> TestCases =
            TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilePath);

        [Test, Timeout(10000*100), TestCaseSource("TestCases")]
        public void TestGetOriginalTransactionsByUTIs(string testFile)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), TestFilePath + testFile);

            TransactionByUTITestCase utiTestCase = TestXmlTool.Deserialize<TransactionByUTITestCase>(filepath);

            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(utiTestCase.InsertXml);

           IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            try
            {
                string result = dealServiceClient.ImportDeal(utiTestCase.InsertXml);
                //Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Assert.Fail("Insert deal failed" + ex.Message);
            }

            XmlNodeList utiIdNodes =
                               insertXmlDocument.SelectNodes("/BulkDealInsert/DealInsert/Transaction/CustomProperties/PropertyGroup[@name='Compliance']/StringProperty[@name='UTI']");

         
            IList<string> insertedUtiIds = new List<string>();

            foreach (XmlNode utilIdNode in utiIdNodes)
            {
                string UtiValue = utilIdNode.Attributes["value"].Value;
                insertedUtiIds.Add(UtiValue);
            }

            if (insertedUtiIds.Count != utiTestCase.ExpectedResult.Length) throw new ArgumentException("Mismatch between inserted transactions and expected result");

            string[] UtiIds = insertedUtiIds.ToArray();

            QaLookUpClient c = new QaLookUpClient();
            List<QaTransactionDTO> resultDtos = c.GetQaTransactionDtosByUti(UtiIds);

            if (resultDtos.Count != utiTestCase.ExpectedResult.Length) throw new ArgumentException("Mismatch between results from GetOriginalTransactionsByUTI and expected result");

            string[] excludeProps = { "TransactionId", "ReferenceData.ModificationDateTimeUtc", "ReferenceData.ReferringId", "ReferenceData.ContractSplitId", "TransactionWorkFlowDetails.TimeStampAuthorised", "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised" };

            for (int i = 0; i < utiTestCase.ExpectedResult.Length; i++)
            {
                QaTransactionDtoAssert.AreEqual(utiTestCase.ExpectedResult[i], resultDtos[i], excludeProps, false);
            }
        }

        [Test]
        public void TestGetOriginalTransactionsByUTI_emptyUTI()
        {
            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();
            try
            {
                lookupServiceClient.GetOriginalTransactionsByUTI(new[] {"", "ComplianceUTI for 103-104" });
            }
            catch (Exception ex)
            {
               Assert.IsTrue(ex.Message.Contains("Parameter name: One of UTIs was either null or empty"));
               return;
            }

            Assert.Fail("Expected to fail");

        }

        [Test]
        public void TestGetOriginalTransactionsByUTI_NonExistentUTI()
        {
            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

            string guid_1 = Guid.NewGuid() + DateTime.Now.ToString();
            string guid_2 = Guid.NewGuid() + DateTime.Now.ToString();
            string guid_3 = Guid.NewGuid() + DateTime.Now.ToString();
            string guid_4 = Guid.NewGuid() + DateTime.Now.ToString();

            string[] utis = new[]
            {
               guid_1, guid_2, guid_3, guid_4
            };

            try
            {
                 TransactionDTO[] resultDTOs = lookupServiceClient.GetOriginalTransactionsByUTI(utis);
                 Assert.AreEqual(0, resultDTOs.Length, "Actual results are not equal expected.");
           
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected to pass but failed: " + ex.Message);
            }

        }
    }
}
