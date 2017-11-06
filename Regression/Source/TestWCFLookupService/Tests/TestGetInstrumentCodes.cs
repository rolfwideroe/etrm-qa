using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;
using TestWCFLookupService.TestCases;

namespace TestWCFLookupService.Tests
{
    public class TestGetInstrumentCodes
    {
       private const string TestFilePath = "Testfiles\\InstrumentCodes\\";

        private static readonly IEnumerable<string> TestCases = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilePath);

        [Test, Timeout(10000 * 100), TestCaseSource("TestCases")]
        public void TestCodes(string testFile)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), TestFilePath + testFile);

            InstrumentCodesTestCase codesTestCase = TestXmlTool.Deserialize<InstrumentCodesTestCase>(filepath);

            XmlDocument insertXmlDocument = new XmlDocument();
            insertXmlDocument.LoadXml(codesTestCase.InsertXml);


            XmlNodeList externalIdNodes = insertXmlDocument.GetElementsByTagName("ExternalId");

            IList<string> insertedExternalIds = new List<string>();

            foreach (XmlNode externalIdNode in externalIdNodes)
            {
                insertedExternalIds.Add(externalIdNode.InnerText);
            }

            //XmlNodeList executionVenues = insertXmlDocument.GetElementsByTagName("ExecutionVenue");

            //string executionVenue = executionVenues[0].InnerText;
            //string testExecVenue = string.Empty;

            //if (string.IsNullOrWhiteSpace(executionVenue))
            //{
            //    XmlNodeList testExecVenues = insertXmlDocument.GetElementsByTagName("TestExecutionVenueName");
            //    testExecVenue = testExecVenues[0].InnerText;

            //}

            if(insertedExternalIds.Count!=codesTestCase.ExpectedResult.Length) throw new ArgumentException("Mismatch between inserted transactions and expected result");

            DealServiceClient dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            LookupServiceClient lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();


            IDictionary<string, int> d = DealInsertTools.InsertXmlReturnTransId(codesTestCase.InsertXml,
                dealServiceClient, false);

            foreach (ExpectedTransaction expectedTransaction in codesTestCase.ExpectedResult)
            {
                string extpectedExternalId = expectedTransaction.ExternalId;

                if(!d.ContainsKey(extpectedExternalId)) throw new ArgumentException("Expected ExternalId : "+extpectedExternalId+" was not inserted");

                int transactionId = d[extpectedExternalId];

                InstrumentCodes[] codesFromTransactions= lookupServiceClient.GetInstrumentCodesByTransactionIds(new[] {transactionId}, codesTestCase.TestExecutionVenue);

                if (codesFromTransactions.Length == 0) Assert.Fail("GetInstrumentCodesByTransactionIds Did not return any instrumentcodes");
                if (codesFromTransactions.Length > 1) Assert.Fail("GetInstrumentCodesByTransactionIds Returned more than one instrumentcode");

                AssertInstrumentCodes(expectedTransaction, codesFromTransactions[0], "GetInstrumentCodesByTransactionIds");

                InstrumentCodes[] codesFromExternalId = lookupServiceClient.GetInstrumentCodesByTransactionExternalIds(new[] { extpectedExternalId }, codesTestCase.TestExecutionVenue);

                if (codesFromExternalId.Length == 0) Assert.Fail("GetInstrumentCodesByTransactionExternalIds Did not return any instrumentcodes");
                if (codesFromExternalId.Length != 2) Assert.Fail("GetInstrumentCodesByTransactionExternalIds Returned did not return two transactions");
    
                AssertInstrumentCodes(expectedTransaction, codesFromExternalId[0], "GetInstrumentCodesByTransactionExternalIds");     
                AssertInstrumentCodes(expectedTransaction, codesFromExternalId[1], "GetInstrumentCodesByTransactionExternalIds");     

            }

      
        }

        private void AssertInstrumentCodes(ExpectedTransaction expectedTransaction, InstrumentCodes actualInstrumentCodes,string method)
        {
            string startErrorMessage = "Method " + method + " Failed for ExternalId : "+expectedTransaction.ExternalId;

           
            

            if (expectedTransaction.ExternalId != actualInstrumentCodes.ExternalId) Assert.Fail("ExternalId for " + method + " Does not match " + expectedTransaction.ExternalId + " ,but Actual was" + actualInstrumentCodes.ExternalId);

            IList<InstrumentCode> actualCodes = actualInstrumentCodes.Codes.ToList();
            IList<ExpectedCode> expectedCodes = expectedTransaction.ExpectedCodes.ToList();

            if (actualCodes.Count != expectedCodes.Count) Assert.Fail(startErrorMessage+", Expected instrument to return "+expectedCodes.Count+" code(s) but was "+actualCodes.Count);

            foreach (ExpectedCode expectedCode in expectedCodes)
            {
                string expectedType = expectedCode.CodeType;

                InstrumentCode actualCode = actualCodes.SingleOrDefault(x => x.CodeType == expectedType);

                if(actualCode==null) Assert.Fail(startErrorMessage+" Expected CodeType : "+expectedType+" was not returned");

                Assert.AreEqual(expectedCode.Code,actualCode.Code,startErrorMessage);
            }


        }

        [Test]
        public void TestGetProductionFacilities()
        {
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            ProductionFacility[] allAvailableFacilities = serviceLookup.GetProductionFacilities();

            foreach (ProductionFacility productinFacility in allAvailableFacilities)
            {
                Console.WriteLine(productinFacility.Name);
            }

        }

    }
}
