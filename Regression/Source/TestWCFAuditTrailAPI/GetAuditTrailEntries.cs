using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;
using Action = ElvizTestUtils.LookUpServiceReference.Action;

namespace TestWCFAuditTrailAPI
{
    [TestFixture]
    public class GetAuditTrailEntries
    {
        readonly string newExternalID = Guid.NewGuid().ToString();
      
        [OneTimeSetUp]
            public void RunBeforeAnyTests()
            {
                Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            }

        [Test]
        public void TestGetAuditTrailEntriesWithNullParams()
        {
            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

            Criteria crit = new Criteria();
            crit.EntityType = EntityType.Transaction;

            try
            {
                AuditTrailEntry[] result = lookupServiceClient.GetAuditTrailEntries(crit);
                Assert.IsNotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void TestGetAuditTrailEntriesAdd()
        {
            string testFile = "PassElectricityFSD_Daily.xml";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);

            AuditTrailTestCasesClass testCase = TestXmlTool.Deserialize<AuditTrailTestCasesClass>(testFilePath);

            if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");
            
            string insertXmlWithNewExternalId = GenerateInsertStringExternalId(testCase.InsertXml);
            
            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(insertXmlWithNewExternalId);

            bool isBulkInsert = false;
            XmlNodeList bulkNode = insertXmlDocument.GetElementsByTagName("BulkDealInsert");

            if (bulkNode.Count > 0)
                isBulkInsert = true;

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

            DateTime fromDateUTC = DateTime.UtcNow.AddSeconds(-1);
            Console.WriteLine(fromDateUTC);

            IDictionary<string, int> transIds = DealInsertTools.InsertXmlReturnTransId(insertXmlWithNewExternalId, dealServiceClient, isBulkInsert);
            if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");
            int dealId;
            if (transIds.ContainsKey(newExternalID))
            {
                dealId = transIds[newExternalID];
                Console.WriteLine(dealId);
            }
            else throw new ArgumentException("No Transactions were found with ExternalId = " + newExternalID);
            //client and server are not synchronized, ticket for GroupIt created.
            Thread.Sleep(5000);

            Criteria auditCriteria = new Criteria
            {
                Action = Action.Add,
                ChangeFromTime = fromDateUTC,
                ChangeUntilTime = DateTime.UtcNow,
                EntityType = EntityType.Transaction,
                EntityId = dealId
            };

            try
            {
                AuditTrailEntry[] result = lookupServiceClient.GetAuditTrailEntries(auditCriteria);
                Assert.IsNotEmpty(result);
                Assert.AreEqual(1, result.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [Test]
        public void TestGetAuditTrailEntriesUpdate()
        {
            string testFile = "El-Forward.xml";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);

            AuditTrailTestCasesClass testCase = TestXmlTool.Deserialize<AuditTrailTestCasesClass>(testFilePath);
            
            //inserting deal
            if (string.IsNullOrEmpty(testCase.InsertXml)) Assert.Fail("Wrong or missing XmlInsertNode");

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            IDictionary<string, int> insTransIds = DealInsertTools.InsertXmlReturnTransId(testCase.InsertXml, dealServiceClient, false);
            if (insTransIds.Count == 0) throw new ArgumentException("No Transactions were inserted");

            if (string.IsNullOrEmpty(testCase.UpdateXml)) Assert.Fail("Wrong or missing XmlUpdateNode");

            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(testCase.UpdateXml);
            Thread.Sleep(3000);

            DateTime fromDateUTC = DateTime.UtcNow.AddSeconds(-2);
            Console.WriteLine(fromDateUTC);

            IDictionary<string, int> transIds = DealInsertTools.InsertXmlReturnTransId(testCase.UpdateXml, dealServiceClient, false);
            
            if (transIds.Count == 0) throw new ArgumentException("No Transactions were inserted");
            KeyValuePair<string, int> first = transIds.First();
            int dealId = first.Value;
            Console.WriteLine(dealId);
            
            DateTime toDateUTC = DateTime.UtcNow.AddSeconds(7);//works bad if From and To date are almost same, some server-client time difference are on place. GrouIT has ticked, not fixed properly
            Console.WriteLine(toDateUTC);
            
           ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();

            Criteria auditCriteria = new Criteria
            {
                Action = Action.Update,
                ChangeFromTime = fromDateUTC,
                ChangeUntilTime = toDateUTC,
                EntityType = EntityType.Transaction,
                EntityId = dealId
            };

            try
            {
                AuditTrailEntry[] result = lookupServiceClient.GetAuditTrailEntries(auditCriteria);
                Assert.AreEqual(1, result.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

       // [Test]
        private string GenerateInsertStringExternalId(String insertxml)
        {
            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(insertxml);

            XmlNodeList externalIdNode = insertXmlDocument.GetElementsByTagName("ExternalId");
            if (externalIdNode.Count == 1)
            {
                XmlNode extId = externalIdNode.Item(0);
                extId.InnerText = newExternalID;
               // Console.WriteLine(insertXmlDocument.OuterXml);
            }
            else Assert.Fail("Error when generating new External Id");

            return (insertXmlDocument.OuterXml);
        }

    }
}
