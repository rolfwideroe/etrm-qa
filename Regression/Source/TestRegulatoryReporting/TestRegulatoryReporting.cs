using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Xml;

namespace TestRegulatoryReporting
{
    [TestFixture]
    public class TestRegulatoryReporting
    {
        private static readonly IEnumerable<string> TestFilesRegulatoryReporting = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        //private string Path2EmirServer = "localhost";
        private string Path2EmirServer = "NETVS-QA161A02.nortest.bradyplc.com";
        private const string DealServiceUrl = "http://{0}:8009/DealService";

        private DealServiceClient GetDealServiceServiceProxy()
        {
            BasicHttpBinding binding = WCFClientUtil.GetDefaultBasicHttpBinding();

            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(DealServiceUrl, Path2EmirServer);

            return new DealServiceClient(binding, address);
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesRegulatoryReporting")]
        public void TestRegulatoryReportingTestFromXmlFile(string testFile)
        {
            Thread.Sleep(5000);

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            string resultFileDirectory = ElvizInstallationUtility.AcerXmlFolder(Path2EmirServer);

            bool isEmir = testFile.Contains("Emir");
            if (isEmir)
                resultFileDirectory = ElvizInstallationUtility.RegisTRXmlFolder(Path2EmirServer);

            XmlDocument doc = new XmlDocument();
            doc.Load(testFilePath);

            XmlNodeList dealNode = doc.GetElementsByTagName("TestData");
            if (dealNode.Count != 1)
                Assert.Fail("Missing TestData");

            XmlNodeList regulatoryReportingNode = doc.GetElementsByTagName("RegulatoryReporting");
            XmlNodeList regulatoryReportingModificationNode = doc.GetElementsByTagName("RegulatoryReportingModification");
            if (regulatoryReportingNode.Count != 1)
                Assert.Fail("Missing resultData");

            string dealInsertResult = "";
            DateTime timeAtDealImport = DateTime.Now;

            XmlNodeList executionNode = doc.GetElementsByTagName("execution");
            string uti = null;
            if (executionNode.Count == 1)
            {
                XmlNodeList periodNode = doc.GetElementsByTagName("period");

                string period = periodNode[0].InnerText;
                ConnectionFactory factory = new ConnectionFactory("failover:tcp://localhost:61616");
                IConnection connection = factory.CreateConnection();
                connection.Start();
                ISession session = connection.CreateSession();
                IDestination destination = SessionUtil.GetDestination(session, "ElvizDealEventsTopic", DestinationType.Topic);
                IMessageProducer producer = session.CreateProducer(destination);
                producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                ITextMessage message = session.CreateTextMessage("<?xml version=\"1.0\" encoding=\"utf-8\"?><NotificationInfo xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Type=\"Execution\" DealId=\"\" Period=\"" + period + "\"/>");

                producer.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.FromSeconds(300));

                producer.Dispose();
                connection.Stop();
                connection.Close();
                connection.Dispose();

            }
            else
            {
                //harcoded to use QA161A02
                if (isEmir)
                {
                    // update UTI to something random, so we get a new deal each time

                    Random random = new Random();
                    int newUti = random.Next(10000);
                    XmlNode customProperties = doc.GetElementsByTagName("CustomProperties").Item(0)?.FirstChild.FirstChild;
                    if (customProperties?.Attributes?.Count == 2)
                    {
                        if (customProperties.Attributes[0].Value == "UTI")
                            customProperties.Attributes[1].Value += newUti;
                    }

                    XmlNode referenceDate = doc.GetElementsByTagName("ExternalId").Item(0);
                    if (referenceDate?.InnerText != null)
                        referenceDate.InnerText += newUti;

                    uti = customProperties?.Attributes?[1].Value;
                }
                else
                    uti = doc.GetElementsByTagName("CustomProperties").Item(0)?.FirstChild.ChildNodes[1].Attributes?[1].Value;
                RunDealInsert(dealInsertResult, dealNode);
            }


            string resultFilePath = CheckResultFilePath(resultFileDirectory, timeAtDealImport, uti);

            XmlDocument newDocument = new XmlDocument();
            newDocument.Load(resultFilePath);

            CompareResult(newDocument, regulatoryReportingNode);

            if (isEmir)
            {
                RunDealInsert(dealInsertResult, dealNode);

                timeAtDealImport = DateTime.Now;
                resultFilePath = CheckResultFilePath(resultFileDirectory, timeAtDealImport, uti);
                newDocument = new XmlDocument();
                newDocument.Load(resultFilePath);

                CompareResult(newDocument, regulatoryReportingModificationNode);

            }
        }

        private void RunDealInsert(string dealInsertResult, XmlNodeList dealNode)
        {
            IDealService dealServiceClient = GetDealServiceServiceProxy();

            try
            {
                dealInsertResult = dealServiceClient.ImportDeal(dealNode[0].InnerXml);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            //create and read XML with ImportDeal's results
            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(dealInsertResult);

            XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
            string dealinsertResult = resultNode[0].InnerText;

            Assert.AreEqual(1, resultNode.Count, "Result Message is not valid, it has multiple results");

            if (dealinsertResult.ToUpper() != "SUCCESS")
            {
                string actualErrorString = "";
                if (resultXml.GetElementsByTagName("Message").Count > 0)
                    actualErrorString = resultXml.GetElementsByTagName("Message")[0].InnerText;

                Assert.Fail("Expected Success, but was Failure: " + " \n ErrorMessage : " + actualErrorString);
            }
        }

        private void CompareResult(XmlDocument newDocument, XmlNodeList regulatoryReportingNode)
        {
            IList<string> newNodes = new List<string>();
            IList<KeyValuePair<string, string>> newTextNodes = new List<KeyValuePair<string, string>>();
            IList<string> expectedNodes = new List<string>();
            IList<KeyValuePair<string, string>> expectedTextNodes = new List<KeyValuePair<string, string>>();

            FindAllNodes(newDocument.ChildNodes[0], newNodes, newTextNodes);
            XmlNode xmlNode = regulatoryReportingNode.Item(0);
            if (xmlNode != null)
                FindAllNodes(xmlNode.ChildNodes[0], expectedNodes, expectedTextNodes);

            if (newNodes.Count != expectedNodes.Count)
            {
                for (int i = 0; i < expectedNodes.Count; i++)
                {
                    if (expectedNodes[i] != newNodes[i])
                    {
                        Assert.Fail(string.Format("Expected node <{0}>, but was <{1}>", expectedNodes[i], newNodes[i]));
                    }
                }
                Assert.Fail("There are differences, perhaps a node is missing");
            }
            else
            {
                for (int i = 0; i < expectedTextNodes.Count; i++)
                {
                    if (expectedTextNodes[i].Key != newTextNodes[i].Key)
                    {
                        Assert.Fail(string.Format("Expected node <{0}>, but was <{1}>", expectedTextNodes[i].Key,
                            newTextNodes[i].Key));
                    }

                    if (expectedTextNodes[i].Key == "actionType")
                        continue;

                    if (expectedTextNodes[i].Key.Contains("Time") && !expectedTextNodes[i].Key.Contains("Quantities"))
                    {
                        string expectedTime = expectedTextNodes[i].Value.Substring(0, 8);
                        string actualTime = newTextNodes[i].Value.Substring(0, 8);
                        Assert.AreEqual(expectedTime, actualTime,
                            string.Format("Expected node value for <{0}> is '{1}', but was '{2}'", expectedTextNodes[i].Key,
                                expectedTime, actualTime));
                        continue;
                    }

                    if (expectedTextNodes[i].Value != newTextNodes[i].Value && expectedTextNodes[i].Value.Length < 100)
                    {
                        Assert.Fail(string.Format("Expected node value for <{0}> is '{1}', but was '{2}'",
                            expectedTextNodes[i].Key, expectedTextNodes[i].Value, newTextNodes[i].Value));
                    }
                }
            }
        }

        private static string CheckResultFilePath(string resultFileDirectory, DateTime timeAtDealImport, string uti)
        {
            string resultFilePath = "";
            int timeout = 25000;
            Stopwatch sw = Stopwatch.StartNew();
            int numberOfResultFiles = 0;
            while (sw.ElapsedMilliseconds < timeout)
            {
                string[] resultFiles = new string[0];
                if (Directory.Exists(resultFileDirectory))
                    resultFiles = Directory.GetFiles(resultFileDirectory);

                numberOfResultFiles = resultFiles.Length;
                foreach (string resultFile in resultFiles)
                {
                    if ((Directory.GetLastWriteTime(resultFile) - timeAtDealImport) > TimeSpan.Zero)
                    {
                        XmlDocument newDocument = new XmlDocument();
                        newDocument.Load(resultFile);
                        XmlNode utiNode = newDocument.GetElementsByTagName("tradeId").Item(0);
                        string foundUti = utiNode?.InnerText;
                        if (string.IsNullOrEmpty(foundUti))
                        {
                            utiNode = newDocument.GetElementsByTagName("uniqueTransactionIdentifier").Item(0)?.ChildNodes[1];
                            foundUti = utiNode?.InnerText;
                        }

                        if (string.IsNullOrEmpty(foundUti))
                        {
                            utiNode = newDocument.GetElementsByTagName("contractId").Item(0);
                            foundUti = utiNode?.InnerText;
                        }


                        if (foundUti == uti)
                        {
                            resultFilePath = resultFile;
                            timeout = 0;
                            break;
                        }
                    }
                }

                Thread.Sleep(1000);
            }

            if (string.IsNullOrEmpty(resultFilePath))
                Assert.Fail(string.Format("No result file found, either timeout or something went wrong: {0}; {1}",
                    resultFileDirectory, numberOfResultFiles));
            return resultFilePath;
        }

        private void FindAllNodes(XmlNode node, IList<string> allNodes, IList<KeyValuePair<string, string>> allTextNodes)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                if (!unwantedTags.Contains(n.Name))
                {
                    allNodes.Add(n.Name);
                    if (n.ChildNodes.Count == 1)
                        allTextNodes.Add(new KeyValuePair<string, string>(n.Name, n.FirstChild.InnerText));
                }
                FindAllNodes(n, allNodes, allTextNodes);
            }
        }

        private readonly List<string> unwantedTags = new List<string>
        {
            @"#significant-whitespace",
            @"#text",
            @"messageId",
            @"creationTimestamp",
            @"tradeId",
            @"modificationDate"
        };

        //private void FindAllNodes2(XmlNode node)
        //{
        //    foreach (XmlNode n in node.ChildNodes)
        //    {
        //        if (!n.Name.Equals(@"#significant-whitespace") && !n.Name.Equals(@"#text"))
        //        {
        //            expectedNodes.Add(n.Name);
        //            if (n.ChildNodes.Count == 1)
        //                expectedTextNodes.Add(new KeyValuePair<string, string>(n.Name, n.FirstChild.InnerText));
        //        }
        //        FindAllNodes2(n);
        //    }
        //}
    }
}
