using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;
using TestWCFDealServiceSetPropertyValues.TestWCFDealServiceSimpleSerialization;
using TestWCFDealServiceSetPropertyValues;

namespace TestWCFDealServiceSetGroupeField
{
    public class SetGroupField
    {

        [TestFixture]
        public class TestSetGroupValues
        {
            [OneTimeSetUp]
            public void RunBeforeAnyTests()
            {
                Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            }

            private TestCase DeserializeXml(string testFilepath)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestCase));

                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFilepath);

                FileStream readFileStream = File.Open(
                    filepath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                return (TestCase)xmlSerializer.Deserialize(readFileStream);
            }
            private static readonly IEnumerable<string> TestFilesSetDealGroup = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesSetDealGroup")]
            public void SetDealGroup(string testFilepath)
            {
                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
                
                TestCase test = this.DeserializeXml(testFilepath);
                int id = test.transactionID;
                int[] ids = {id, 107};
                bool res1 = service.SetGroupField(ids, GroupFields.GroupField1, test.GroupField1);
                Assert.AreEqual( true, res1);
               
                bool res2 = service.SetGroupField(ids, GroupFields.GroupField2, test.GroupField2);
                Assert.AreEqual(true, res2);
                
                bool res3 = service.SetGroupField(ids, GroupFields.GroupField3, test.GroupField3);
                Assert.AreEqual(true, res3);

                ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);
                Assert.AreEqual(2, transactions.Length);
                Assert.AreEqual(test.GroupField1, transactions[0].GroupField1,
                                "Wrong result for GroupField1:");
                Assert.AreEqual(test.GroupField2, transactions[0].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(test.GroupField3, transactions[0].GroupField3,
                                "Wrong result for GroupField3:");
                Assert.AreEqual(test.GroupField1, transactions[1].GroupField1,
                             "Wrong result for GroupField1:");
                Assert.AreEqual(test.GroupField2, transactions[1].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(test.GroupField3, transactions[1].GroupField3,
                                "Wrong result for GroupField3:");

            }
            [Test] 
            public void SetDealGroupSimple()
            {
                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();

                int[] ids = { 123 };
                bool res1 = service.SetGroupField(ids, GroupFields.GroupField1, "test.GroupField1" );
                Assert.AreEqual(true, res1);

                bool res2 = service.SetGroupField(ids, GroupFields.GroupField2, "test.GroupField2");
                Assert.AreEqual(true, res2);

                bool res3 = service.SetGroupField(ids, GroupFields.GroupField3, "test.GroupField3");
                Assert.AreEqual(true, res3);

                ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);

                Assert.AreEqual(1, transactions.Length);
                Assert.AreEqual("test.GroupField1", transactions[0].GroupField1,
                                "Wrong result for GroupField1:");
                Assert.AreEqual("test.GroupField2", transactions[0].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual("test.GroupField3", transactions[0].GroupField3,
                                "Wrong result for GroupField3:");

            }
            [Test] 
            public void SetDealGroupNullValue()
            {
                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
                
                int transactionID = 121;

                string propertyGroupName = "Deal Group";
                string propertyName = "GroupField3";
                string propertyValue = "api test SetValueForGroupProperty test";

                TestSetPropertyValues.SetPropertyValue(transactionID, propertyGroupName, propertyName, propertyValue);
                
                int[] ids = { transactionID };
                bool res1 = service.SetGroupField(ids, GroupFields.GroupField1, null);
                Assert.AreEqual(true, res1);
                
                bool res2 = service.SetGroupField(ids, GroupFields.GroupField2, null);
                Assert.AreEqual(true, res2);

                bool res3 = service.SetGroupField(ids, GroupFields.GroupField3, null);
                Assert.AreEqual(true, res3);

                ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);

                Assert.AreEqual(1, transactions.Length);
                Assert.AreEqual(null, transactions[0].GroupField1,
                              "Wrong result for GroupField1:");
                Assert.AreEqual(null, transactions[0].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(null, transactions[0].GroupField3,
                                "Wrong result for GroupField3:");

            }

            [Test]
            public void SetDealGroupEmptyValue()
            {
                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
        
                int[] ids = { 121, 123 };
                bool res1 = service.SetGroupField(ids, GroupFields.GroupField1, "");
                Assert.AreEqual(true, res1);

                bool res2 = service.SetGroupField(ids, GroupFields.GroupField2, "");
                Assert.AreEqual(true, res2);

                bool res3 = service.SetGroupField(ids, GroupFields.GroupField3, "");
                Assert.AreEqual(true, res3);

                ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);

                Assert.AreEqual(2, transactions.Length);
                Assert.AreEqual(null, transactions[0].GroupField1,
                              "Wrong result for GroupField1:");
                Assert.AreEqual(null, transactions[0].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(null, transactions[0].GroupField3,
                                "Wrong result for GroupField3:");

                Assert.AreEqual(null, transactions[1].GroupField1,
                              "Wrong result for GroupField1:");
                Assert.AreEqual(null, transactions[1].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(null, transactions[1].GroupField3,
                                "Wrong result for GroupField3:");
            }

            [Test]
            public void SetDealGroupStringEmpty()
            {
                IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
          
                int[] ids = { 121, 123 };
                bool res1 = service.SetGroupField(ids, GroupFields.GroupField1, string.Empty);
                Assert.AreEqual(true, res1);

                bool res2 = service.SetGroupField(ids, GroupFields.GroupField2, string.Empty);
                Assert.AreEqual(true, res2);

                bool res3 = service.SetGroupField(ids, GroupFields.GroupField3, string.Empty);
                Assert.AreEqual(true, res3);

                ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
                TransactionDTO[] transactions = serviceLookup.GetTransactionsByIds(ids);

                Assert.AreEqual(2, transactions.Length);
                Assert.AreEqual(null, transactions[0].GroupField1,
                              "Wrong result for GroupField1:");
                Assert.AreEqual(null, transactions[0].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(null, transactions[0].GroupField3,
                                "Wrong result for GroupField3:");

                Assert.AreEqual(null, transactions[1].GroupField1,
                              "Wrong result for GroupField1:");
                Assert.AreEqual(null, transactions[1].GroupField2,
                                "Wrong result for GroupField2:");
                Assert.AreEqual(null, transactions[1].GroupField3,
                                "Wrong result for GroupField3:");
            }
        }
    }
}
