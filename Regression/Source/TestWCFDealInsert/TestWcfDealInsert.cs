using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using ElvizTestUtils.DealServiceReference;
using NUnit.Framework;
using ElvizTestUtils;

namespace TestWcfDealInsert
{
    [TestFixture]

    public class TestWcfDealInsert
    {
       // for creating portfolios and new filter
        [TestFixtureSetUp]
        public void Setup()
        {
            ElvizTestUtils.LoadStaticData.LoadStaticData loadStatic = new ElvizTestUtils.LoadStaticData.LoadStaticData();
            loadStatic.InsertPortfolios();
        }

        private static readonly IEnumerable<string> TestFilesDealInsertFromZip = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFilesZip");

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesDealInsertFromZip")]
        public void TestWcfDealInsertTestFrom_ZipFile(string testFile)
        {
            // XmlDocument doc = new XmlDocument();

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFilesZip\\" + testFile);

            using (ZipArchive archive = ZipFile.OpenRead(testFilePath))
            {
                string testFileNameOnly = testFile.Split('.').First();

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name.StartsWith(testFileNameOnly, StringComparison.InvariantCultureIgnoreCase))
                    {

                        Stream entryStream = entry.Open();

                        XmlDocument doc = new XmlDocument();
                        doc.Load(entryStream);

                        string result;

                        IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

                        try
                        {
                            result = dealServiceClient.ImportDeal(doc.InnerXml);
                            //xml = null;
                            doc = null;
                        }
                        catch (Exception ex)
                        {
                            doc = null;
                            GC.Collect();
                            throw new AssertionException(ex.Message);
                        }

                        dealServiceClient = null;
                        //  doc = null;
                        GC.Collect();

                        XmlDocument resultXml = new XmlDocument();
                        resultXml.LoadXml(result);

                        XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");

                        if (resultNode.Count != 1)
                            throw new ArgumentException("Result Message is not valid, it has multiple results");

                        string sucessFailure = resultNode[0].InnerText;

                        if (sucessFailure == "Success") return;

                        XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");

                        string errorMessage = messageNode[0].InnerText;

                        Assert.Fail(errorMessage);
                    }
                }
            }

            //Console.WriteLine(testFilePath);

            //XmlDocument doc = new XmlDocument();
            //  doc.Load(testFilePath);
        }


        private static readonly IEnumerable<string> TestFilesDealInsert =
            TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesDealInsert")]
        public void TestWcfDealInsertTestFromXmlFile(string testFile)
        {
            // XmlDocument doc = new XmlDocument();

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);

            //string xml = File.ReadAllText(testFilePath);
            XmlDocument doc = new XmlDocument();
            doc.Load(testFilePath);

            string result;

            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

            try
            {
                result = dealServiceClient.ImportDeal(doc.InnerXml);
                //xml = null;
                doc = null;
            }
            catch (Exception ex)
            {
                doc = null;
                GC.Collect();
                throw new AssertionException(ex.Message);
            }

            dealServiceClient = null;
            //  doc = null;
            GC.Collect();

            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(result);

            XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");

            if (resultNode.Count != 1)
                throw new ArgumentException("Result Message is not valid, it has multiple results");

            string sucessFailure = resultNode[0].InnerText;

            if (sucessFailure == "Success") return;

            XmlNodeList messageNode = resultXml.GetElementsByTagName("Message");

            string errorMessage = messageNode[0].InnerText;

            Assert.Fail(errorMessage);
        }


    }
}

